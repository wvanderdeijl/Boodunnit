using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationCamera : MonoBehaviour
{
    public float ConversationRadius = 7f;
    private bool _isConversing;
    private CameraController _cameraController;
    private float _angle;
    private float _upperAngle;
    private float _lowerAngle;

    private Vector3 _conversationCenterPoint;

    private void Awake()
    {
        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (ConversationManager.HasConversationStarted && !_isConversing) 
            StartConversing(ConversationManager.ConversationTarget.position);
        else if (_isConversing && !ConversationManager.HasConversationStarted) 
            EndConversing();
    }

    public void StartConversing( Vector3 conversationTarget)
    {
        _isConversing = true;
        DeterminePointInformation(conversationTarget);
        Vector3 cameraPoint = AttemptPerpendicularPoints(0);

        StartCoroutine(CameraMover(cameraPoint, conversationTarget));
    }

    IEnumerator CameraMover(Vector3 cameraPoint, Vector3 conversationTarget)
    {
        transform.position = Vector3.Slerp(transform.position, cameraPoint, Time.deltaTime);
        
        //This makes both the conversation target and the player look at one another
        _cameraController.CameraRotationTarget.transform.LookAt(conversationTarget);
        transform.LookAt(conversationTarget);
        ConversationManager.ConversationTarget.LookAt(_cameraController.CameraRotationTarget);
        yield return null;
        if (Vector3.Distance(transform.position, cameraPoint) > 1f && ConversationManager.HasConversationStarted) 
            StartCoroutine(CameraMover(cameraPoint, conversationTarget));
    }

    bool CheckCameraObstruction(Vector3 cameraPoint, Vector3 conversationTargetPosition)
    {
        Vector3 targetDirection = cameraPoint - _conversationCenterPoint;
        return Physics.Raycast(_conversationCenterPoint, targetDirection, out RaycastHit hit,
            Vector3.Distance(cameraPoint, conversationTargetPosition) + 0.2f, 
            LayerMask.GetMask("Default"));
    }

    private void DeterminePointInformation(Vector3 conversationTarget)
    {
        Vector3 playerPos = _cameraController.CameraRotationTarget.position;
        _conversationCenterPoint = 
            Vector3.Lerp(conversationTarget, playerPos, 0.5f);
        
        Vector3 delta = _conversationCenterPoint - playerPos;
        _angle = Mathf.Atan2(delta.x, delta.z) * 180f / Mathf.PI;
        
        _lowerAngle = _angle - 90;
        _upperAngle = _angle + 90;
        if (_lowerAngle < 0) _lowerAngle += 360;
        if (_upperAngle > 360) _upperAngle -= 360;
    }
    
    public void EndConversing()
    {
        _isConversing = false;
    }

    private Vector3 AttemptPerpendicularPoints(float angleOffset)
    {
        Vector3 leftAttempt = TryPoint(angleOffset, _lowerAngle);
        Vector3 rightattempt = TryPoint(angleOffset, _upperAngle);

        if (angleOffset > 90)
        {
            return _cameraController.CameraRotationTarget.position;
        }
        if (leftAttempt != Vector3.zero)
        {
            return  leftAttempt;
        }
        if (rightattempt != Vector3.zero)
        {
            return rightattempt;
        }
        return AttemptPerpendicularPoints(angleOffset + 1);
    }

    private Vector3 TryPoint(float offset, float originalAngle)
    {
        float currentUpperAngle = CheckAngleOverflow(originalAngle + offset);
        float currentLowerAngle = CheckAngleOverflow(originalAngle - offset);
        
        //Check Left
        Vector3 CameraPoint = CameraController.GetCirclePosition(currentUpperAngle, ConversationRadius);
        if (CheckCameraObstruction(_conversationCenterPoint + CameraPoint, _conversationCenterPoint))
        {
            //Check Right
            CameraPoint = CameraController.GetCirclePosition(currentLowerAngle, ConversationRadius);
            if (CheckCameraObstruction(_conversationCenterPoint + CameraPoint, _conversationCenterPoint)) 
                return Vector3.zero;  
        }
        return _conversationCenterPoint + CameraPoint;
    }

    private float CheckAngleOverflow(float angle)
    {
        if (angle < 0) angle += 360;
        if (angle > 360) angle -= 360;
        return angle;
    }
}