using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ClimbBehaviour : MonoBehaviour
{
    public float MinimumStamina { get; set; }
    public float MaximumStamina { get; set; }
    public float CurrentStamina { get; set; }

    public float StaminaConsumptionPerSecond, StaminaReplenishRatePerSecond;

    public float Speed { get; set; }
    
    public bool IsClimbing;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _rotationSpeed = 100f;
    [SerializeField] private Image _staminaRadialCircle;

    private void Awake()
    {
        if (CurrentStamina > MaximumStamina)
        {
            CurrentStamina = MaximumStamina;
        }
        else if (CurrentStamina < MinimumStamina)
        {
            CurrentStamina = MinimumStamina;
        }
    }

    private void Update()
    {
        if (IsClimbing) CheckSurface();
        if (CurrentStamina <= 0) DisableClimbing();
    }

    //Call this method in an ability function (f.e. UseFirstAbility() in IEntity).
    public void ToggleClimb()
    {
        if (IsClimbing)
        {
            DisableClimbing();
            return;
        }

        EnableClimbing();
    }

    public void Climb(Vector3 direction)
    {
        _rigidbody.angularVelocity = Vector3.zero;
        direction = direction.normalized;
        _rigidbody.velocity = direction * Speed;
        
        if (direction != Vector3.zero)
        {
            Vector3 axis = new Vector3(0f, Input.GetAxisRaw("Horizontal"), 0f);
            transform.Rotate(axis * (_rotationSpeed * Time.deltaTime));
        }
    }

    public void EnableClimbing()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f, 
            LayerMask.GetMask("Climbable")))
        {
            RotateToWallNormal(hit);
            _rigidbody.useGravity = false;
            IsClimbing = true;
            StartCoroutine(DepleteStamina());
        }
    }

    public void DisableClimbing()
    {
        transform.LookAt(transform.position -transform.up);
        _rigidbody.useGravity = true;
        IsClimbing = false;
        StartCoroutine(ReplenishStamina());
    }

    public void CheckSurface()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 3f, 
            LayerMask.GetMask("Climbable")))
        {
            RotateToWallNormal(hit);
            CloseDistanceToWall(hit);
            return;
        }

        DisableClimbing();
    }
    
    public void RotateToWallNormal(RaycastHit hit)
    {
        Quaternion upToWallNormal = Quaternion.FromToRotation(transform.up, hit.normal);
        transform.rotation = upToWallNormal * transform.rotation;
    }

    private void CloseDistanceToWall(RaycastHit hit)
    {
        Vector3 offset = hit.point - transform.position;
        offset -= -transform.up * transform.localScale.y / 2f;
        transform.position += offset;
    }

    private IEnumerator DepleteStamina()
    {
        if (IsClimbing)
        {
            CurrentStamina -= StaminaConsumptionPerSecond * Time.deltaTime;
            _staminaRadialCircle.fillAmount = CurrentStamina / MaximumStamina;

            yield return null;

            if (CurrentStamina > MinimumStamina)
            {
                StartCoroutine(DepleteStamina());
            }
        }
    }

    private IEnumerator ReplenishStamina()
    {
        if (!IsClimbing)
        {
            CurrentStamina += StaminaReplenishRatePerSecond * Time.deltaTime;
            _staminaRadialCircle.fillAmount = CurrentStamina / MaximumStamina;

            yield return null;

            if (CurrentStamina < MaximumStamina)
            {
                StartCoroutine(ReplenishStamina());
            }
        }
    }
}
