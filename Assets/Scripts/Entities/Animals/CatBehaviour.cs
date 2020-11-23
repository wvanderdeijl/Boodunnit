using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public class CatBehaviour : BaseEntity
{
    private void Awake()
    {
        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(RatBehaviour)] = 3f,
            [typeof(PoliceManBehaviour)] = 2f,
            [typeof(VillagerBehaviour)] = 2f,
            [typeof(ILevitateable)] = 5f
        };
    }

    private void Update()
    {
        CheckSurroundings();
    }
    
    public override void UseFirstAbility()
    {
        //TODO first ability.
    }
}
