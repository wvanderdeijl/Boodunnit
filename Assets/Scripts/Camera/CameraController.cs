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
                _elevationOverflow = true;
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
    public Vector3 LookAtTargetPosition;
    private Vector3 oldTargetPosition;

    private Vector3 _pointToSlerpTo;
    [SerializeField]private float _angle;
    [SerializeField] private Vector2 _rotationInput;
    [SerializeField] private float _scrollingInput;

    private bool _scrollZoomActivation;
    private bool _isAligningCamera = false;

    private float _minElevationOrigin;
    private float _maxElevationOrigin;
    public float MaxElevation = 8f;
    public float MinElevation = 0f;

    private Vector3 _collisionZoomPoint;
    private Vector3 _collisionZoomDirection;

    private bool _elevationOverflow;
    [SerializeField] private bool _cameraLock = true;

    [SerializeField] private float _elevationRange = 2f;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _pointToSlerpTo = transform.position;
        _angle = Vector3.Angle(CameraRotationTarget.position, _pointToSlerpTo);
        Distance = MaxDistance;
        _minElevationOrigin = MinElevation;
        _maxElevationOrigin = MaxElevation;
        LookAtTargetPosition = CameraRotationTarget.position;
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
        _pointToSlerpTo.y = ElevationRange;
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

        Vector3 position2DIfied = TwoDIfy(transform.position);
        Vector3 targetPosition2DIfied = TwoDIfy(CameraRotationTarget.position);
        if (Vector3.Distance(position2DIfied, targetPosition2DIfied) > Distance || 
            Vector3.Distance(position2DIfied, targetPosition2DIfied) < Distance) 
            AlignCameraWithTarget();
        Debug.DrawRay(_collisionZoomPoint, _collisionZoomDirection, Color.magenta);
    }

    private void LateUpdate()
    {
        Vector3 direction = (transform.position - CameraRotationTarget.position).normalized;
        float zoomValue = 0;
        if (Physics.Raycast(CameraRotationTarget.position, direction, out RaycastHit raycastHit,
            Distance))
        {
            _collisionZoomDirection = direction;
            _collisionZoomPoint = raycastHit.point -= direction.normalized / 2f;
            zoomValue = (-Vector3.Distance(transform.position, raycastHit.point) - 0.1f);
            _scrollZoomActivation = false;
        }
        else if (!Physics.Raycast(CameraRotationTarget.position, direction, out RaycastHit hit, MaxDistance) && !_scrollZoomActivation)
            zoomValue = 1f;
        
        if (_scrollingInput != 0)
        {
            _scrollZoomActivation = true;
            zoomValue = (_scrollingInput * 5 * Time.deltaTime * RotationSpeed);
        }
        
        ScrollZoom(zoomValue);
    }

    private void AlignCameraWithTarget()
    {
        RotateCamera(0);
    }

    private void ScrollZoom(float zoomAmount)
    {
        Distance += zoomAmount;
        MinElevation = _minElevationOrigin * Distance / 7;
        MaxElevation = _maxElevationOrigin * Distance / 7;
    }

    public void RotateCamera(float input)
    {
        StartCoroutine(RotateCam());
    }
    public IEnumerator RotateCam()
    {
        yield return new WaitForSeconds(0.1f);
        float plusMinusMultiplier = _rotationInput.x > 0 ? 1 : _rotationInput.x < 0 ? -1 : 0;
        float increment = plusMinusMultiplier * ( Math.Abs(_rotationInput.x) / (1f/ RotationSpeed));
        print("increment: " + increment);
        print("plusMinusMultiplier: " + plusMinusMultiplier);
        _angle += increment;
        
        if (_angle > 360) _angle -= 360;
        if (_angle < 0) _angle += 360;

        _pointToSlerpTo = GetCirclePosition( _angle, Distance);
    }

    private void ElevateCamera(float verticalInput)
    {
        StartCoroutine(ElevateCam());
    }

    private IEnumerator ElevateCam()
    {
        yield return new WaitForSeconds(0.1f);
        float plusMinusMultiplier = _rotationInput.x > 0 ? 1 : _rotationInput.x < 0 ? -1 : 0;
        ElevationRange += (0.1f * plusMinusMultiplier) + _rotationInput.y / 20;
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
