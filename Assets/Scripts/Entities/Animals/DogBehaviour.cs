using System;
using System.Collections.Generic;
using Entities;
using Enums;

public class DogBehaviour : BaseEntity
{
    private void Awake()
    {
        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(PoliceManBehaviour)] = 2f,
            [typeof(VillagerBehaviour)] = 2f,
            [typeof(ILevitateable)] = 5f
        };
    }

    public override void UseFirstAbility()
    {
        //TODO First ability
    }
}
