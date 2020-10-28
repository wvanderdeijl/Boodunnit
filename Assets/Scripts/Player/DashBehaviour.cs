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

    private ArrayList _stopDashLayerList;

    private void Awake()
    {
        _rigidbodyPlayer = GetComponent<Rigidbody>();
        _dashSpeed = _dashDistance / _dashDuration;
        _endPositionRadius = GetComponent<Collider>().bounds.extents.z;
        _stopDashLayerList = new ArrayList() {10};
    }

    public void Dash()
    {
        StartCoroutine(PerformDash());
        StartCoroutine(DashCoroutineTimer());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_stopDashLayerList.Contains(collision.gameObject.layer))
        {
            StopCoroutine(PerformDash());
            IsDashing = false;
        }
    }

    private IEnumerator PerformDash()
    {
        IsDashing = true;
        Vector3 oldVelocity = _rigidbodyPlayer.velocity;
        Vector3 newVelocity = transform.rotation * transform.forward * _dashSpeed;
        if (CheckDashEndPosition(newVelocity))
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

    private bool CheckDashEndPosition(Vector3 velocity)
    {
        Vector3 endPosition = velocity * _dashDuration;

        Collider[] endPositionColliders = Physics.OverlapSphere(endPosition, _endPositionRadius);
        if (endPositionColliders != null)
        {
            return endPositionColliders.Length == 0;
        }
        return true;
    }

    private IEnumerator DashCoroutineTimer()
    {
        yield return new WaitForSeconds(_dashCooldown + _dashDuration);
        DashOnCooldown = false;
    }
}
