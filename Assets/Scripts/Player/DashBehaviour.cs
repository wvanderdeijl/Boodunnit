using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DashBehaviour : MonoBehaviour
{
    public bool IsDashing = false;
    public bool DashOnCooldown = false;

    private float _dashCooldown = 2f;
    private float _dashDuration = 0.4f;
    private float _dashDistance = 4f;
    private float _dashSpeed;
    private float _endPositionRadius;

    private Rigidbody _rigidbodyPlayer;

    private IEnumerator _dashCoroutine;

    private void Awake()
    {
        _rigidbodyPlayer = GetComponent<Rigidbody>();
        _dashSpeed = _dashDistance / _dashDuration;
        _endPositionRadius = GetComponent<Collider>().bounds.extents.z;
    }

    public void Dash()
    {
        _dashCoroutine = PerformDash();
        StartCoroutine(_dashCoroutine);
        StartCoroutine(DashTimer());
    }

    private void OnCollisionStay(Collision collision)
    {
        StopCoroutine(_dashCoroutine);
        IsDashing = false;
    }

    private IEnumerator PerformDash()
    {
        IsDashing = true;

        Vector3 oldVelocity = _rigidbodyPlayer.velocity;
        Vector3 newVelocity = transform.forward * _dashSpeed;

        if (CheckDashEndPosition())
        {
            gameObject.layer = 9;
        }

        _rigidbodyPlayer.velocity = newVelocity;

        yield return new WaitForSeconds(_dashDuration);

        _rigidbodyPlayer.velocity = oldVelocity;
        gameObject.layer = 8;

        DashOnCooldown = true;
        IsDashing = false;
    }

    private bool CheckDashEndPosition()
    {
        Vector3 endPosition = transform.position + (transform.forward * _dashDistance);

        Collider[] endPositionColliderArray = Physics.OverlapSphere(endPosition, _endPositionRadius);
        if (endPositionColliderArray != null)
        {
            return endPositionColliderArray.Length == 0;
        }
        return true;
    }

    private IEnumerator DashTimer()
    {
        yield return new WaitForSeconds(_dashCooldown + _dashDuration);
        DashOnCooldown = false;
    }
}
