using System;
using System.Collections;
using System.Collections.Generic;
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

    /// <summary>
    /// Describes the speed at which the camera will move to a given new position along a circular axis
    /// </summary>
    public float Sensitivity = 5f;

    public float MaxDistance = 7;
    public float MinDistance = 1.255763f;

    [SerializeField] private float _distance;

    public float RotationSpeed = 20f;

    public Transform CameraRotationTarget;
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
    [SerializeField] private bool _cameraLock = true;

    [SerializeField] private float _elevationRange = 2f;

    private void Awake()
    {
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
        Cursor.lockState = CursorLockMode.Locked;
        Distance = MaxDistance;
        _minElevationOrigin = MinElevation;
        _maxElevationOrigin = MaxElevation;
        ScrollZoom(-0.0001f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) Cursor.lockState = ToggleLockMode();
        if (_cameraLock)
        {
            _rotationInput.x = Input.GetAxis("Mouse X");
            _rotationInput.y = -Input.GetAxis("Mouse Y");
            _scrollingInput = -Input.GetAxis("Mouse ScrollWheel");
        }
        else
        {
            _rotationInput = Vector2.zero;
            _scrollingInput = 0;
        }

        float angleOffset = 0;
        _pointToSlerpTo.y = ElevationRange + CameraRotationTarget.position.y;

        Debug.DrawRay(transform.position, _pointToSlerpTo - transform.position, Color.cyan);
        if (Vector3.Distance(transform.position, _pointToSlerpTo) > 0.1)
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

    private void LateUpdate()
    {
        AlignCameraWithTarget(); ;
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

    private void ScrollZoom(float zoomAmount)
    {
        Distance += zoomAmount;
        MinElevation = _minElevationOrigin * Distance / 7;
        MaxElevation = _maxElevationOrigin * Distance / 7;
    }

    public void RotateCamera(float rotationInput)
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

    private CursorLockMode ToggleLockMode()
    {
        _cameraLock = !_cameraLock;
        if (_cameraLock) return CursorLockMode.Locked;
        else return CursorLockMode.Confined;
    }

    void AlignCameraWithTarget()
    {
        Vector3 slerpPos = _pointToSlerpTo;
        RotateCamera(Input.GetAxisRaw("Vertical") != 0 ? Input.GetAxis("Horizontal") : 0);
        _pointToSlerpTo = Vector3.Slerp(slerpPos, _pointToSlerpTo, Time.deltaTime * 20 / 6);

    }

}
