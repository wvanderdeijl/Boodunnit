using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

    private System.Random _random = new System.Random();

    public void AddNewEntityArea(EntityArea newArea)
    {
        _areas.Add(newArea);
    }

    public void GetAreaForEntityType(GameObject entity)
    {
        double randomDouble = (_random.NextDouble() * 100);
        foreach (EntityArea area in _areas)
        {
            foreach (EntitiesAllowed allowedEntities in area.EntitiesAllowedInThisArea)
            {
                string nameOfEntity = allowedEntities.NameOfEntity.ToLower();
                if (nameOfEntity.Equals(entity.name.ToLower()))
                {
                    if (randomDouble < allowedEntities.ChanceThatEntityPicksThisArea)
                    {
                        area.AddEntityToArea(entity);
                        entity.transform.position = new Vector3(area.transform.position.x, entity.transform.position.y, area.transform.position.z);
                        return;
                    }
                }
            }
        }
    }
}