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

    public float MaxDistance = 7;
    public float MinDistance = 1.255763f;
    public float Distance
    {
        get { return _distance; }
        set
        {
            if (value > MaxDistance) value = MaxDistance;
            if (value < MinDistance) value = MinDistance;
            _distance = value;
        }
    }
    [SerializeField]
    private float _distance;
    
    public float RotationSpeed = 20f;

    public Transform CameraRotationTarget;
    
    private Vector3 _pointToSlerpTo;
    private float _angle;
    private Vector2 _rotationInput;
    [SerializeField]
    private float _scrollingInput;

    private bool _scrollZoomActivation;

    private float _minElevationOrigin;
    private float _maxElevationOrigin;
    public float MaxElevation = 8f;
    public float MinElevation = -0.5f;
    public float ElevationRange
    {
        get { return _elevationRange;}
        set
        {
            if (value > MaxElevation) value = MaxElevation;
            if (value < MinElevation) value = MinElevation;
            _elevationRange = value;
        }
    }
    [SerializeField]
    private float _elevationRange = 2f;

    private void Awake()
    {
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
        Cursor.lockState = CursorLockMode.Locked;
        Distance = MaxDistance;
        _minElevationOrigin = MinElevation;
        _maxElevationOrigin = MaxElevation;
    }

    private void Update()
    {
        _rotationInput.x = Input.GetAxis("Mouse X");
        _rotationInput.y = -Input.GetAxis("Mouse Y");
        _scrollingInput = -Input.GetAxis("Mouse ScrollWheel");
        
        float angleOffset = 0;
        _pointToSlerpTo.y = ElevationRange + CameraRotationTarget.position.y;;

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

        if (_scrollingInput != 0)
        {
            _scrollZoomActivation = true;
            ScrollZoom(_scrollingInput * 5 * Time.deltaTime * RotationSpeed);
        }
        else if (!_scrollZoomActivation && Distance < MaxDistance)
        {
            ScrollZoom(1f);
        }
    }

    private void LateUpdate()
    {
        if (Physics.Raycast(CameraRotationTarget.position, (transform.position - CameraRotationTarget.position).normalized,out RaycastHit raycastHit,
            Distance ))
        {
            ScrollZoom(-Vector3.Distance(transform.position, raycastHit.point));
            _scrollZoomActivation = false;
            if (raycastHit.point.y <= CameraRotationTarget.position.y || CameraRotationTarget.position.y > transform.position.y)
            {
                ElevateCamera(30f);
                Vector3 newPoint = raycastHit.point;
                newPoint.y += 0.1f;
                transform.position = newPoint;
                _scrollZoomActivation = true;
            }
            else if (raycastHit.point.y > CameraRotationTarget.position.y +
                CameraRotationTarget.GetComponent<Collider>().bounds.size.y)
            {
                ElevateCamera(-30f);
                _scrollZoomActivation = true;
            }
        }
    }

    private void ScrollZoom(float zoomAmount)
    {
        Distance += zoomAmount;
        MinElevation = _minElevationOrigin * Distance / 7;
        MaxElevation = _maxElevationOrigin * Distance / 7;
        RotateCamera(0);
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
