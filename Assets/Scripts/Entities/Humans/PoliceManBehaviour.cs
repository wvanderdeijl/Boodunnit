using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;

public class PoliceManBehaviour : BaseEntity
{
    [SerializeField][Range(0, 10)] private float _donutDetectionRadius = 10f;
    [SerializeField][Range(0, 360)] private float _donutDetectionAngle = 90f;

    private void Awake()
    {
        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(ILevitateable)] = 3f
        };
    }
    
    private void Update()
    {
        if (!IsPossessed && !ConversationManager.hasConversationStarted) MoveWithPathFinding();
        CheckDonutsInSurrounding();
    }

    private void CheckDonutsInSurrounding()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _donutDetectionRadius);
        
        foreach (Collider hitCollider in hitColliders)
        {
            RaycastHit hit;

            Vector3 fromPosition = transform.position;
            Vector3 toPosition = hitCollider.transform.position;
            Vector3 direction = toPosition - fromPosition;
            
            float donutAngle = Vector3.Angle(direction, transform.forward);

            if (Physics.Raycast(fromPosition, direction, out hit, _donutDetectionRadius))
            {
                Donut isDonut = hit.collider.gameObject.GetComponent<Donut>();
                
                if (isDonut)
                {
                    if (donutAngle > -(_donutDetectionAngle / 2) && donutAngle < _donutDetectionAngle / 2)
                    {
                        Debug.Log("I SEE: " + hit.transform.gameObject.name);
                    }
                }
            }
        }
    }

    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (!ConversationManager.hasConversationStarted) base.MoveEntityInDirection(direction);
    }

    public override void UseFirstAbility()
    {
        //TODO implement PoliceManBehaviour.
    }
}
