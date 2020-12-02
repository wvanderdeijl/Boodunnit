using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class DashBehaviour : MonoBehaviour
{
    public bool IsDashing = false;
    public bool DashOnCooldown = false;

    public float _dashCooldown = 2f;
    public float _dashDuration = 0.4f;
    public float _dashDistance = 4f;
    private float _dashSpeed;
    private float _endPositionRadius;

    private Rigidbody _rigidbodyPlayer;

    private IEnumerator _dashCoroutine;

    private float _distanceBooliaDashable;
    private float _dashableThickness;
    private float _distanceDashableEndPosition;

    private void Awake()
    {
        _rigidbodyPlayer = GetComponent<Rigidbody>();
        _dashSpeed = _dashDistance / _dashDuration;
        _endPositionRadius = GetComponent<Collider>().bounds.extents.z * 0.9f;
    }

    public void Dash()
    {
        _dashCoroutine = PerformDash();
        StartCoroutine(_dashCoroutine);
        StartCoroutine(DashTimer());
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal;
        Vector3 vel = _rigidbodyPlayer.velocity;

        if (Vector3.Angle(vel, -normal) < 50 && IsDashing && !DashOnCooldown)
        {
            StopCoroutine(_dashCoroutine);
            StopDash();
        }
    }

    private IEnumerator PerformDash()
    {
        IsDashing = true;

        _rigidbodyPlayer.useGravity = false;

        Vector3 oldVelocity = _rigidbodyPlayer.velocity;
        Vector3 newVelocity = transform.forward * _dashSpeed;

        InitializeDashDistances();
        if (_distanceDashableEndPosition > 1 && CheckDashEndPosition())
        {
            gameObject.layer = 9;
        }

        _rigidbodyPlayer.velocity = newVelocity;

        yield return new WaitForSeconds(_dashDuration);

        oldVelocity.y = 0;
        _rigidbodyPlayer.velocity = oldVelocity;
        gameObject.layer = 8;

        StopDash();
    }
    private void StopDash()
    {
        DashOnCooldown = true;
        IsDashing = false;
        _rigidbodyPlayer.useGravity = true;
    }

    private bool CheckDashEndPosition()
    {
        Vector3 endPosition = transform.position + (transform.forward * _dashDistance);

        Collider[] endPositionColliderArray = Physics.OverlapSphere(endPosition, _distanceDashableEndPosition);

        if (endPositionColliderArray != null)
        {
            bool canDash = true;
            foreach (Collider collider in endPositionColliderArray)
            {
                Vector3 endOFDashablePosition = transform.position + (transform.forward * (_dashableThickness + _distanceBooliaDashable));
                float distanceColliderDashable = Vector3.Distance(collider.transform.position, endOFDashablePosition);
                Debug.Log(distanceColliderDashable);
                canDash = distanceColliderDashable > 1.5;
                if (!canDash)
                {
                    break;
                }
            }
            return canDash;
        }
        return true;
    }

    private void InitializeDashDistances()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 10))
        {
            float angle = Vector3.Angle(hitInfo.normal, -transform.forward);
            float cosAngle = Math.Abs(Mathf.Cos(angle));
            float size = hitInfo.transform.localScale.z;
            _dashableThickness = size / cosAngle;

            _distanceBooliaDashable = hitInfo.distance;

            _distanceDashableEndPosition = _dashDistance - _dashableThickness - _distanceBooliaDashable;
        }
    }

    private IEnumerator DashTimer()
    {
        float currentTime = 0;
        float interval = _dashCooldown + _dashDuration;

        while (currentTime < interval)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }
        DashOnCooldown = false;
    }
}
