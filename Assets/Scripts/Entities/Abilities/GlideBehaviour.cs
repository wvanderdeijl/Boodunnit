using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class GlideBehaviour : MonoBehaviour
{
    public bool IsGliding { get; set; }

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _glideGravity = 1f;

    private void Update()
    {
        _rigidbody.angularVelocity = Vector3.zero;
        if(IsGliding) ApplyLoweredGravity();
    }

    public void ToggleGlide()
    {
        if (!IsGliding)
        {
            EnableGlide();
            return;
        }
        DisableGlide();
    }

    private void EnableGlide()
    {
        IsGliding = true;
        _rigidbody.useGravity = false;
    }

    private void DisableGlide()
    {
        IsGliding = false;
        _rigidbody.useGravity = true;
    }

    private void ApplyLoweredGravity()
    {
        _rigidbody.velocity =  new Vector3(_rigidbody.velocity.x, -_glideGravity, _rigidbody.velocity.z);
    }
}
