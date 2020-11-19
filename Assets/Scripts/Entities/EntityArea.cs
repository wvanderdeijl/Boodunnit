using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityArea : MonoBehaviour
{
    public List<EntitiesAllowed> EntitiesAllowedInThisArea;
    public List<GameObject> EntitiesInArea = new List<GameObject>();

    public int MinimumEntitiesRequiredInArea;
    public int MaximumEntitiesAllowedInArea;

    private void Awake()
    {
        // Any time a new area is added, we will add it to the area list in the EntityAreaHandler.
        EntityAreaHandler.Instance.AddNewEntityArea(this);
    }

    /// <summary>
    /// Add entity to the area he is assigned to.
    /// </summary>
    /// <param name="entity">Entity that will be added to a area.</param>
    public void AddEntityToArea(GameObject entity)
    {
        foreach(EntitiesAllowed allowedEntity in EntitiesAllowedInThisArea)
        {
            string nameOfEntity = allowedEntity.NameOfEntity.ToLower();
            if (nameOfEntity.Equals(entity.name.ToLower()))
            {
                EntitiesInArea.Add(entity);
            }
        }
    }

    /// <summary>
    /// Check if the area is not full.
    /// </summary>
    /// <returns>Whether the area is full or not.</returns>
    public bool CheckIfAreaIsNotFull()
    {
        return EntitiesInArea.Count < MaximumEntitiesAllowedInArea;
    }

    /// <summary>
    /// Remove entity from current area
    /// </summary>
    /// <param name="entity">Entity to remove from area</param>
    public void RemoveEntityFromArea(GameObject entity)
    {
        if (EntitiesInArea.Contains(entity))
            EntitiesInArea.Remove(entity);
    }

    public int GetEntityTimeInArea(GameObject entity)
    {
        foreach (EntitiesAllowed allowedEntity in EntitiesAllowedInThisArea)
        {
            if (entity.name.ToLower().Equals(allowedEntity.NameOfEntity.ToLower()))
                return allowedEntity.EntityTimeSpentInAreaInSeconds;
        }

        return default;
    }

    private void OnTriggerStay(Collider other)
    {
        GameObject collidedObject = other.gameObject;
        if (EntitiesInArea.Contains(collidedObject))
        {
            BaseMovement baseMovementOfCollidedObject = collidedObject.GetComponent<BaseMovement>();
            if (!baseMovementOfCollidedObject.IsOnCountdown)
            {
                baseMovementOfCollidedObject.IsOnCountdown = true;
                StartCoroutine(baseMovementOfCollidedObject.StartCountdownInArea(GetEntityTimeInArea(collidedObject)));
            }
        }
    }
}