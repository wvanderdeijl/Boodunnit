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
        IsScaredOfLevitatableObject = true;
        ScaredOfEntities = new Dictionary<CharacterType, float>()
        {
            [CharacterType.PoliceMan] = 2f,
            [CharacterType.Villager] = 2f,
        };
    }
    
    public override void UseFirstAbility()
    {
        //TODO first ability.
    }
}
