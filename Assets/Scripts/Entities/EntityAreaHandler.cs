using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityAreaHandler
{
    private List<EntityArea> _areas = new List<EntityArea>();

    private static EntityAreaHandler _instance;
    public static EntityAreaHandler Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new EntityAreaHandler();
            }
            return _instance;
        }
    }

    public System.Random Random = new System.Random();

    /// <summary>
    /// Add a new area to the EntityAreaHandler.
    /// </summary>
    /// <param name="newArea">Area you want to add.</param>
    public void AddNewEntityArea(EntityArea newArea)
    {
        _areas.Add(newArea);
    }

    /// <summary>
    /// Give entity a random area that is available for him.
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <returns>Area that entity can move to.</returns>
    public EntityArea GetRandomAreaForEntity(GameObject entity)
    {
        List<EntityArea> allowedAreasForEntity = GetAvailableAreasAndTotalAreaWeight(entity, out int totalAreaWeight);
        int randomAreaNumber = Random.Next(0, totalAreaWeight);

        foreach (EntityArea area in allowedAreasForEntity)
        {
            foreach(EntitiesAllowed allowedEntity in area.EntitiesAllowedInThisArea)
            {
                if(allowedEntity.NameOfEntity.ToLower() == entity.name.ToLower() && (randomAreaNumber -= allowedEntity.OdssEntityPicksThisAreaInWeight) < 0)
                {
                    area.EntitiesInArea.Add(entity);
                    return area;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Searchs for all areas available if entity is allowed to go there, and gets the totalWeight of all available areas to him.
    /// </summary>
    /// <param name="nameOfEntity">Name of the entity</param>
    /// <param name="totalWeight">Total weight of the areas, which will also be returned</param>
    /// <returns>List of all allowed areas</returns>
    private List<EntityArea> GetAvailableAreasAndTotalAreaWeight(GameObject entity, out int totalWeight)
    {
        totalWeight = 0;
        List<EntityArea> areasAvailable = new List<EntityArea>();

        foreach (EntityArea area in _areas)
        {
            foreach (EntitiesAllowed allowedEntities in area.EntitiesAllowedInThisArea)
            {
                string nameOfEntity = entity.name.ToLower();
                if (allowedEntities.NameOfEntity.ToLower().Equals(nameOfEntity))
                {
                    area.RemoveEntityFromArea(entity);
                    if (area.CheckIfAreaIsNotFull())
                    {
                        areasAvailable.Add(area);
                        totalWeight += allowedEntities.OdssEntityPicksThisAreaInWeight;
                    }
                }
            }
        }
        return areasAvailable;
    }

    /// <summary>
    /// Get a random position in the area.
    /// </summary>
    /// <param name="area">Area assigned to player</param>
    /// <param name="entity">Just to get the entity's y position</param>
    /// <returns>Random position</returns>
    public Vector3 GetRandomPositionInArea(EntityArea area, GameObject entity)
    {
        BoxCollider collider = area.GetComponent<BoxCollider>();
        if (collider)
        {
            Bounds bounds = collider.bounds;
            return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                bounds.min.y,
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
        }
        return default;
    }
}