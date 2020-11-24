using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationCamera : MonoBehaviour
{
    public bool IsConversing;
    private CameraController _cameraController;
    private float _angle;
    private float _upperAngle;
    private float _lowerAngle;
    public float ConversationRadius = 7f;

    private Vector3 _conversationCenterPoint;

    private void Awake()
    {
        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (ConversationManager.hasConversationStarted && !IsConversing) 
            StartConversation(ConversationManager.ConversationTarget.position);
        else if (IsConversing && !ConversationManager.hasConversationStarted) 
            EndConversation();
    }

    public void StartConversation( Vector3 conversationTarget)
    {
        IsConversing = true;
        DeterminePointInformation(conversationTarget);
        Vector3 cameraPoint = Attempt(0);

        StartCoroutine(CameraMover(cameraPoint, conversationTarget));
    }

    IEnumerator CameraMover(Vector3 cameraPoint, Vector3 conversationTarget)
    {
        transform.position = Vector3.Slerp(transform.position, cameraPoint, Time.deltaTime);
        _cameraController.CameraRotationTarget.transform.LookAt(conversationTarget);
        transform.LookAt(conversationTarget);
        ConversationManager.ConversationTarget.LookAt(_cameraController.CameraRotationTarget);
        yield return null;
        if (Vector3.Distance(transform.position, cameraPoint) > 1f && ConversationManager.hasConversationStarted) 
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
        _angle = (Mathf.Atan2(delta.x, delta.z) * 180 / Mathf.PI);
        
        _lowerAngle = _angle - 90;
        _upperAngle = _angle + 90;
        if (_lowerAngle < 0) _lowerAngle += 360;
        if (_upperAngle > 360) _upperAngle -= 360;
    }
    
    public void EndConversation()
    {
        IsConversing = false;
    }

    private Vector3 Attempt(float offset)
    {
        Vector3 leftAttempt = TryPoint(offset, _lowerAngle);
        Vector3 rightattempt = TryPoint(offset, _upperAngle);

        if (offset > 90)
        {
            print("obstructed");
            return _cameraController.CameraRotationTarget.position;
        }
        if (leftAttempt != Vector3.zero)
        {
            print("left, offset: " + offset);
            return  leftAttempt;
        }
        if (rightattempt != Vector3.zero)
        {
            print("right, offset: " +offset);
            return rightattempt;
        }
        return Attempt(offset + 1);
    }

    private Vector3 TryPoint(float offset, float originalAngle)
    {
        float currentUpperAngle = CheckAngleOverflow(originalAngle + offset);
        float currentLowerAngle = CheckAngleOverflow(originalAngle - offset);

        Vector3 CameraPoint = CameraController.GetCirclePosition(currentUpperAngle, ConversationRadius);
        if (CheckCameraObstruction(_conversationCenterPoint + CameraPoint, _conversationCenterPoint))
        {
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