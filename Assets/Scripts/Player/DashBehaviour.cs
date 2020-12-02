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

    public float DashCooldown = 2f;
    public float DashDuration = 0.2f;
    public float DashDistance = 7f;
    private float _dashSpeed;

    private Rigidbody _rigidbodyPlayer;

    private IEnumerator _dashCoroutine;

    private float _distanceBooliaDashable;
    private float _dashableThickness;
    private float _distanceDashableEndPosition;

    private void Awake()
    {
        _rigidbodyPlayer = GetComponent<Rigidbody>();
        _dashSpeed = DashDistance / DashDuration;
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
        if (CheckDashEndPosition())
        {
            gameObject.layer = 9;
        }

        _rigidbodyPlayer.velocity = newVelocity;

        yield return new WaitForSeconds(DashDuration);

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
        Vector3 endPosition = transform.position + (transform.forward * DashDistance);

        Collider[] endPositionColliderArray = Physics.OverlapSphere(endPosition, _distanceDashableEndPosition);

        bool canDash = true;

        if (endPositionColliderArray != null)
        {
            foreach (Collider collider in endPositionColliderArray)
            {
                if (collider.gameObject.layer != 10)
                {
                    Vector3 endOFDashablePosition = transform.position + (transform.forward * (_dashableThickness + _distanceBooliaDashable));
                    float distanceColliderDashable = Vector3.Distance(collider.transform.position, endOFDashablePosition);

                    if (distanceColliderDashable < 2)
                    {
                        canDash = false;
                        break;
                    }
                }
            }
        }
        return canDash;
    }

    private void InitializeDashDistances()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, DashDistance))
        {
            float angle = Vector3.Angle(hitInfo.normal, -transform.forward);
            float cosAngle = Math.Abs(Mathf.Cos(angle));
            float size = hitInfo.transform.localScale.z;
            _dashableThickness = size / cosAngle;

            _distanceBooliaDashable = hitInfo.distance;

            _distanceDashableEndPosition = DashDistance - _dashableThickness - _distanceBooliaDashable;
        }
    }

    private IEnumerator DashTimer()
    {
        float currentTime = 0;
        float interval = DashCooldown + DashDuration;

        while (currentTime < interval)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }
        DashOnCooldown = false;
    }
}
