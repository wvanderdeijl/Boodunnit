using System;
using System.Collections.Generic;
using Entities;
using Enums;
using UnityEngine;

public class RatBehaviour : BaseEntity
{
    private ClimbBehaviour _climbBehaviour;

    private void Awake()
    {
        InitBaseEntity();
        _climbBehaviour = GetComponent<ClimbBehaviour>();
        CanJump = true;

        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(BirdBehaviour)] = 5f,
            [typeof(VillagerBehaviour)] = 4f,
            [typeof(PoliceManBehaviour)] = 4f,
            [typeof(ILevitateable)] = 3f
        };
        
        _climbBehaviour.MinimumStamina = 0f;
        _climbBehaviour.MaximumStamina = 50f;
        _climbBehaviour.CurrentStamina = 50f;
        _climbBehaviour.Speed = 5f;
    }

    Shop _shop;
    bool boughtHat = false;

    private void LateUpdate()
    {
        if (_climbBehaviour.StaminaBarCanvas) _climbBehaviour.StaminaBarCanvas.enabled = IsPossessed;

        if (!IsPossessed && !ConversationManager.HasConversationStarted) CheckForShops();

        if (!boughtHat && _shop && Vector3.Distance(transform.position, _shop.transform.position) < 2)
        {
            GameObject item = _shop.BuyRandomItem();
            item.transform.position = transform.Find("Hat Position (Empty GO)").position;
            item.transform.parent = transform.Find("Hat Position (Empty GO)");
            item.transform.localScale = new Vector3(1, 1, 1);
            item.transform.rotation = transform.Find("Hat Position (Empty GO)").rotation;
            _shop = null;
            ResetDestination();
            boughtHat = true;
        }
    }
    private void CheckForShops()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100);

        foreach (Collider hitCollider in hitColliders)
        {
            RaycastHit hit;

            Vector3 fromPosition = transform.position;
            Vector3 toPosition = hitCollider.transform.position;
            Vector3 direction = toPosition - fromPosition;

            float shopAngle = Vector3.Angle(direction, transform.forward);

            if (Physics.Raycast(fromPosition, direction, out hit, 100))
            {
                Shop isShop = hit.collider.gameObject.GetComponent<Shop>();

                if (isShop)
                {
                    if (shopAngle > -(90 / 2) && shopAngle < 90 / 2)
                    {
                        Debug.Log("IK ZIE EEN SHOP: " + hit.transform.name);
                        GoToShop(isShop);
                        return;
                    }
                }
            }
        }
    }

    private void GoToShop(Shop shop)
    {
        _shop = shop;
        TargetToFollow = shop.gameObject;
        ChangePathFindingState(PathFindingState.Following);
    }

    public override void MoveEntityInDirection(Vector3 direction)
    {
        if (_climbBehaviour.IsClimbing) _climbBehaviour.Climb();
        else base.MoveEntityInDirection(direction);
    }

    public override void UseFirstAbility()
    {
        _climbBehaviour.ToggleClimb();
    }
}
