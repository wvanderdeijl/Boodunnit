using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using Random = System.Random;

public class RatBehaviour : BaseMovement, IEntity
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed = 5f;
    
    public Random Random = new Random();
    public Vector3 Target;
    public bool IsMoving;
    
    private void Awake()
    {
        Rigidbody = _rigidbody;
        Speed = _speed;
    }

    private void Update()
    {
        if(!IsMoving) MoveEntityInDirection(FindPath());
        if (_rigidbody.velocity == Vector3.zero) IsMoving = false; 
    }

    private Vector3 FindPath()
    {
        IsMoving = true;
        float randX = Random.Next(-6,6);
        float randZ = Random.Next(-6,6);
        return Target = new Vector3(randX, 0, randZ);
    }

    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }
    public void DealFearDamage(float amount)
    {
        throw new NotImplementedException();
    }

    public IEnumerator CalmDown()
    {
        throw new NotImplementedException();
    }

    public void Faint()
    {
        throw new NotImplementedException();
    }

    public void CheckSurroundings()
    {
        throw new NotImplementedException();
    }

    public void UseFirstAbility()
    {
        //Climbv
    }

    public void UseSecondAbility()
    {
        throw new NotImplementedException();
    }
}