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
        InitBaseEntity();

        ScaredOfEntities = new Dictionary<CharacterType, float>()
        {
            [CharacterType.Rat] = 3f,
        };
    }

    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (!ConversationManager.HasConversationStarted) base.MoveEntityInDirection(direction);
    }

    public override void UseFirstAbility()
    {
        //TODO Villager first ability.
    }
}
