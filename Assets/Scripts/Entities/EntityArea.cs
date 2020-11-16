using System.Collections.Generic;
using UnityEngine;

public class EntityArea : MonoBehaviour
{
    public List<EntitiesAllowed> EntitiesAllowedInThisArea;
    public List<GameObject> EntitiesInArea;

    public int MinimumEntitiesRequiredInArea;
    public int MaximumEntitiesAllowedInArea;

    private void Awake()
    {
        EntitiesInArea = new List<GameObject>();
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
}