using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;

public class RatBehaviour : BaseMovement, IEntity, IPossessable
{
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }

    [SerializeField] private ClimbBehaviour _climbBehaviour;

    private void Awake()
    {
        _climbBehaviour.MinimumStamina = 0f;
        _climbBehaviour.MaximumStamina = 10f;
        _climbBehaviour.CurrentStamina = 10f;
        _climbBehaviour.Speed = 5f;
    }

    private void Update()
    {
        //if (_climbBehaviour.IsClimbing) _climbBehaviour.Climb(direction);
        //else MoveEntityInDirection(direction);
    }

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
        _climbBehaviour.ToggleClimb();
    }

    public void UseSecondAbility()
    {
        throw new NotImplementedException();
    }
}
