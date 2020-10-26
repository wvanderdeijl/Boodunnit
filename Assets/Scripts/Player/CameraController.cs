using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Sensitivity = 1f;
    public float Distance = 7f;
    public float RotationSpeed = 50f;

    public Transform CameraRotationTarget;
    [SerializeField]
    private Vector3 _pointToSlerpTo;
    [SerializeField]
    private float _angle;
    private int _rotationInput;
    
    private void Awake()
    {
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
    }

    private void Update()
    {
        _rotationInput = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
        if (_rotationInput == 0)
        {
            _pointToSlerpTo = transform.position;
            _angle = Vector3.Angle(CameraRotationTarget.position, transform.position);
        }
    }

    private void LateUpdate()
    {
        if (Vector3.Distance(transform.position, _pointToSlerpTo) > 1)
        {
            Vector3 slerpedPosition = Vector3.Slerp(transform.position, _pointToSlerpTo, Time.deltaTime * Sensitivity);
            slerpedPosition.y = transform.position.y;
            transform.position = slerpedPosition;
            transform.LookAt(CameraRotationTarget);
        }

        if (_rotationInput != 0)
        {
            RotateCamera(_rotationInput);
        }
    }

    private void RotateCamera(float rotationInput)
    {
        _angle += (rotationInput * RotationSpeed * Time.deltaTime);
        if (_angle > 360) _angle -= 360;
        if (_angle < 0) _angle += 360;

        _pointToSlerpTo = GetCirclePosition(CameraRotationTarget.position, _angle, Distance);
    }
    
    private Vector3 GetCirclePosition(Vector3 circlePosition, float angle, float radius)
    {
        angle *= (float) (Math.PI / 180f);
        float newX = (float) ((float) circlePosition.x + (radius * Math.Sin(angle)));
        float newZ = (float) ((float) circlePosition.z + (radius * Math.Cos(angle)));
        return new Vector3(newX, circlePosition.y, newZ);
    }
}
