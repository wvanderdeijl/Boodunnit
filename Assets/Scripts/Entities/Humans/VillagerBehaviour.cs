using Enums;
using System;
using System.Collections.Generic;
using Entities;
using UnityEngine;
using UnityEngine.AI;

public class VillagerBehaviour : BaseEntity
{
    void Awake()
    {
        // Todo give Name and Profession

        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(RatBehaviour)] = 3f,
            [typeof(ILevitateable)] = 3f
        };
    }

    void Update()
    {
        CheckSurroundings();
    }

    public override void UseFirstAbility()
    {
        //TODO Villager first ability.
    }
}
