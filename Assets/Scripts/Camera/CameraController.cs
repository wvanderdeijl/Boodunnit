using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Numerics;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
                // ScrollZoom(_rotationInput.y * Time.deltaTime);
            }

            _elevationRange = value;
        }
    }

    /// <summary>
    /// Describes the speed at which the camera will move to a given new position along a circular axis
    /// </summary>
    public float Sensitivity = 5f;

    public float MaxDistance = 7;
    public float MinDistance = 1.255763f;

    [SerializeField] private float _distance;

    public float RotationSpeed = 20f;

    public Transform CameraRotationTarget;
    public Vector3 LookAtTargetPosition;
    private Vector3 oldTargetPosition;

    private Vector3 _pointToSlerpTo;
    private float _angle;
    private Vector2 _rotationInput;
    [SerializeField] private float _scrollingInput;

    private bool _scrollZoomActivation;
    private bool _isAligningCamera = false;

    private float _minElevationOrigin;
    private float _maxElevationOrigin;
    public float MaxElevation = 8f;
    public float MinElevation = 0f;

    private bool _elevationOverflow;
    private bool _isAligning;
    [SerializeField] private bool _cameraLock = true;

    [SerializeField] private float _elevationRange = 2f;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
        Cursor.lockState = CursorLockMode.Locked;
        Distance = MaxDistance;
        _minElevationOrigin = MinElevation;
        _maxElevationOrigin = MaxElevation;
        LookAtTargetPosition = CameraRotationTarget.position;
        ScrollZoom(-0.01f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) Cursor.lockState = ToggleLockMode();
        if (_cameraLock)
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
        _pointToSlerpTo.y = ElevationRange + CameraRotationTarget.position.y;
        float SensitivityMultiplier = Vector3.Distance(transform.position, _pointToSlerpTo);
        if (SensitivityMultiplier > 1.1f)
        {
            Vector3 slerpedPosition = Vector3.Slerp(
                transform.position, 
                CameraRotationTarget.position + _pointToSlerpTo, 
                Time.deltaTime * Sensitivity * SensitivityMultiplier
                );
            transform.position = slerpedPosition;
            transform.LookAt(CameraRotationTarget);
        }

        if (_rotationInput.x != 0)
        {
            transform.LookAt(CameraRotationTarget);
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

        if (Vector3.Distance(TwoDIfy(transform.position), TwoDIfy(CameraRotationTarget.position)) > Distance || 
            Vector3.Distance(TwoDIfy(transform.position), TwoDIfy(CameraRotationTarget.position)) < Distance) 
            AlignCameraWithTarget();
    }

    private void LateUpdate()
    {
        if (_scrollingInput != 0)
        {
            _scrollZoomActivation = true;
            ScrollZoom(_scrollingInput * 5 * Time.deltaTime * RotationSpeed);
        }
        else if (!_scrollZoomActivation && Distance < MaxDistance)
        {
            ScrollZoom(1f);
        }
        Vector3 direction = (transform.position - CameraRotationTarget.position).normalized;
        if (Physics.Raycast(CameraRotationTarget.position, direction, out RaycastHit raycastHit,
            Distance))
        {
            ScrollZoom(-Vector3.Distance(transform.position, raycastHit.point));
            _scrollZoomActivation = false;
        }
    }

    private void AlignCameraWithTarget()
    {
        _isAligning = true;
        RotateCamera(0);
    }

    private void ScrollZoom(float zoomAmount)
    {
        Distance += zoomAmount;
        MinElevation = _minElevationOrigin * Distance / 7;
        MaxElevation = _maxElevationOrigin * Distance / 7;
    }

    public void RotateCamera(float rotationInput)
    {
        _isAligning = false;
        _angle += (rotationInput * RotationSpeed * Time.deltaTime);
        if (_angle > 360) _angle -= 360;
        if (_angle < 0) _angle += 360;

        _pointToSlerpTo = GetCirclePosition( _angle, Distance);
    }

    private void ElevateCamera(float verticalInput)
    {
        ElevationRange += verticalInput * Time.deltaTime;
    }

    private Vector3 GetCirclePosition(float angle, float radius)
    {
        Vector3 circlePosition = Vector3.zero;
        angle *= (float) (Math.PI / 180f);
        float newX = (float) ((float) circlePosition.x + (radius * Math.Sin(angle)));
        float newZ = (float) ((float) circlePosition.z + (radius * Math.Cos(angle)));
        return new Vector3(newX, circlePosition.y, newZ);
    }

    private CursorLockMode ToggleLockMode()
    {
        _cameraLock = !_cameraLock;
        if (_cameraLock) return CursorLockMode.Locked;
        else return CursorLockMode.Confined;
    }

    private Vector3 TwoDIfy(Vector3 input)
    {
        input.y = 0;
        return input;
    }

}
