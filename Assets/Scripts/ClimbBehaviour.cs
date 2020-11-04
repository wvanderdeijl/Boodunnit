using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbBehaviour : MonoBehaviour
{

    public float MinimumStamina { get; set; }
    public float MaximumStamina { get; set; }
    public float CurrentStamina { get; set; }
    public float Speed { get; set; }
    
    public bool IsClimbing;
    public bool CanClimb;
    
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _wallDetectionRadius = 2f;

    private void Awake()
    {
        IsClimbing = false;
    }

    //Call this method in an ability function (f.e. UseFirstAbility() in IEntity).
    public void ToggleClimb()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f,
            LayerMask.NameToLayer("Climbable"));
        
        //TODO rotate transform.down towards wall
        IsClimbing = !IsClimbing;
    }

    public void Climb(Vector3 direction)
    {
        direction = direction.normalized;
        _rigidbody.velocity = direction * Speed;
    }
    
    
}
