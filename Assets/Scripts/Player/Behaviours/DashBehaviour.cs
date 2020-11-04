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

    public Image DashCooldownImage;

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

        if (CheckDashEndPosition())
        {
            gameObject.layer = 9;
        }

        _rigidbodyPlayer.velocity = newVelocity;

        yield return new WaitForSeconds(_dashDuration);

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

        Collider[] endPositionColliderArray = Physics.OverlapSphere(endPosition, _endPositionRadius);

        if (endPositionColliderArray != null)
        {
            return endPositionColliderArray.Length == 0;
        }
        return true;
    }

    private IEnumerator DashTimer()
    {
        float currentTime = 0;
        float interval = _dashCooldown + _dashDuration;

        while (currentTime < interval)
        {
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
            DashCooldownImage.fillAmount = currentTime / interval;
        }
        DashOnCooldown = false;
    }
}