﻿using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float Distance
    {
        get { return _distance; }
        set
        {
            if (value > MaxDistance)
                value = MaxDistance;
            if (value < MinDistance)
                value = MinDistance;
            _distance = value;
        }
    }

    public float ElevationRange
    {
        get { return _elevationRange; }
        set
        {
            if (value > MaxElevation) value = MaxElevation;
            if (value < MinElevation)
            {
                value = MinElevation;
                ScrollZoom(_rotationInput.y * Time.deltaTime);
            }

            _elevationRange = value;
        }
    }
    [Header("Target & Distance")]
    public float MaxDistance = 7;
    public float MinDistance = 1.255763f;

    private float _distance;
    public float RotationSpeed = 1f;

    public Transform CameraRotationTarget;
    /// <summary>
    /// DO NOT reasign  this. only meant for reading by other classes.
    /// To reassign use non static public Transform CameraRotationTarget
    /// </summary>
    public static Transform RotationTarget;

    private Vector3 _pointToSlerpTo;
    private float _angle;
    private Vector2 _rotationInput;
    private float _scrollingInput;
    private bool _scrollZoomActivation;
    
    private float _minElevationOrigin;
    private float _maxElevationOrigin;
    [Header("Elevation")]
    public float MaxElevation = 8f;
    public float MinElevation = -0.5f;
    private float _elevationRange = 2f;

    [Header("Disable-ables")] 
    public bool CanScrollZoom;
    public bool CanAutoZoom;

    private void Awake()
    {
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
        Distance = MaxDistance;
        _minElevationOrigin = MinElevation;
        _maxElevationOrigin = MaxElevation;  

    }

    private void Update()
    {
        if (ConversationManager.HasConversationStarted) return;
        RotationTarget = CameraRotationTarget;
        
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            GameManager.CursorIsLocked = !GameManager.CursorIsLocked;
        }
    
        if (GameManager.CursorIsLocked)
        {
            _rotationInput.x = Input.GetAxisRaw("Mouse X");
            _rotationInput.y = -Input.GetAxisRaw("Mouse Y");
            _scrollingInput = -Input.GetAxisRaw("Mouse ScrollWheel");
        }
        else
        {
            _rotationInput = Vector2.zero;
            _scrollingInput = 0;
        }

        float angleOffset = 0;
        _pointToSlerpTo.y = ElevationRange;

        transform.position = CameraRotationTarget.position + _pointToSlerpTo;
        transform.LookAt(CameraRotationTarget);

        if (_rotationInput.x != 0)
        {
            transform.LookAt(CameraRotationTarget);
            RotateCamera();
        }
        else
        {
            _angle -= angleOffset;
        }

        if (_rotationInput.y != 0)
        {
            if (!LevitateBehaviour.IsRotating)
            {
                ElevationRange += (_rotationInput.y / 10f);
            }
        }

        Vector3 position2DIfied = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPosition2DIfied = new Vector3(CameraRotationTarget.position.x, 0, CameraRotationTarget.position.z);

        if (Vector3.Distance(position2DIfied, targetPosition2DIfied) > Distance || 
            Vector3.Distance(position2DIfied, targetPosition2DIfied) < Distance) 
            AlignCameraWithTarget();
    }

    private void LateUpdate()
    {
        Vector3 direction = (transform.position - CameraRotationTarget.position).normalized;
        float zoomValue = 0;
        if (CanAutoZoom)
        {if (Physics.Raycast(CameraRotationTarget.position, direction, out RaycastHit raycastHit,
                Distance, LayerMask.GetMask("Default")))
            {
                raycastHit.point -= direction.normalized / 2f;
                zoomValue = (-Vector3.Distance(transform.position, raycastHit.point) - 0.1f);
                _scrollZoomActivation = false;
            }
            else if (!Physics.Raycast(CameraRotationTarget.position, direction, out RaycastHit hit, MaxDistance, 
                LayerMask.GetMask("Default")) && !_scrollZoomActivation)
                zoomValue = 1f;
        }
        
        if (CanScrollZoom && _scrollingInput != 0)
        {
            _scrollZoomActivation = true;
            zoomValue = _scrollingInput * 3;
        }
        
        ScrollZoom(zoomValue);
    }

    private void AlignCameraWithTarget()
    {
        RotateCamera();
    }

    private void ScrollZoom(float zoomAmount)
    {
        Distance += zoomAmount;
        MinElevation = _minElevationOrigin * Distance / 7;
        MaxElevation = _maxElevationOrigin * Distance / 7;
    }

    public void RotateCamera()
    {
        if (LevitateBehaviour.IsRotating) { return; }

        float plusMinusMultiplier = _rotationInput.x > 0 ? 1 : _rotationInput.x < 0 ? -1 : 0;
        float increment = plusMinusMultiplier * (Mathf.Abs(_rotationInput.x) / (1f/ RotationSpeed));
        _angle += increment;
        
        if (_angle > 360) _angle -= 360;
        if (_angle < 0) _angle += 360;

        _pointToSlerpTo = GetCirclePosition( _angle, Distance);
    }

    public static Vector3 GetCirclePosition(float angle, float radius)
    {
        Vector3 circlePosition = Vector3.zero;
        angle *= Mathf.PI / 180f;
        float newX = circlePosition.x + (radius * Mathf.Sin(angle));
        float newZ = circlePosition.z + (radius * Mathf.Cos(angle));
        return new Vector3(newX, circlePosition.y, newZ);
    }
}
