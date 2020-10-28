using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    /// <summary>
    /// Describes the speed at which the camera will move to a given new position along a circular axis
    /// </summary>
    public float Sensitivity = 5f; 
    public float Distance = 7f;
    public float RotationSpeed = 20f;

    public Transform CameraRotationTarget;
    
    private Vector3 _pointToSlerpTo;
    private float _angle;
    private Vector2 _rotationInput;
    
    public float ElevationRange
    {
        get { return _elevationRange;}
        set
        {
            if (value > 8f) value = 8f;
            if (value < -2f) value = -2f;
            _elevationRange = value;
        }
    }
    private float _elevationRange = 2f;

    private void Awake()
    {
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _rotationInput.x = Input.GetAxis("Mouse X");
        _rotationInput.y = -Input.GetAxis("Mouse Y");
        
        float angleOffset = 0;
        _pointToSlerpTo.y = ElevationRange;

        Vector3 camPos = transform.position;
        Vector2 plainCamPos = new Vector2(camPos.x, camPos.z);
        Vector3 targetPos = CameraRotationTarget.position;
        Vector2 plainTargetPos = new Vector2(targetPos.x, targetPos.z);
        
        if (Vector2.Distance(plainCamPos, plainTargetPos) > Distance + 5f)
        {
            RotateCamera(0);
        }
        
        if (Vector3.Distance(transform.position, _pointToSlerpTo) > 0.5)
        {
            Vector3 slerpedPosition = Vector3.Slerp(transform.position, _pointToSlerpTo, Time.deltaTime * Sensitivity);
            transform.position = slerpedPosition;
            transform.LookAt(CameraRotationTarget);
            angleOffset = _rotationInput.x * RotationSpeed * Time.deltaTime * Sensitivity;
        }

        if (_rotationInput.x != 0)
        {
            RotateCamera(_rotationInput.x);
        }
        else
        {
            _angle -= angleOffset;
        }

        if (_rotationInput.y != 0)
        {
            ElevateCamera(_rotationInput.y);
        }
    }


    private void RotateCamera(float rotationInput)
    {
        _angle += (rotationInput * RotationSpeed * Time.deltaTime);
        if (_angle > 360) _angle -= 360;
        if (_angle < 0) _angle += 360;

        _pointToSlerpTo = GetCirclePosition(CameraRotationTarget.position, _angle, Distance);
    }

    private void ElevateCamera(float verticalInput)
    {
        ElevationRange += verticalInput * Time.deltaTime;
    }

    private Vector3 GetCirclePosition(Vector3 circlePosition, float angle, float radius)
    {
        angle *= (float) (Math.PI / 180f);
        float newX = (float) ((float) circlePosition.x + (radius * Math.Sin(angle)));
        float newZ = (float) ((float) circlePosition.z + (radius * Math.Cos(angle)));
        return new Vector3(newX, circlePosition.y, newZ);
    }
}
