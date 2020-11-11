using Interfaces;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        EntityAreaHandler.Instance.AddNewEntityArea(this);
    }

    public void AddEntityToArea(GameObject entity)
    {
        bool isEntityAllowed = false;
        foreach(EntitiesAllowed allowedEntity in EntitiesAllowedInThisArea)
        {
            string nameOfEntity = allowedEntity.NameOfEntity.ToLower();
            if (nameOfEntity.Equals(entity.name.ToLower()))
            {
                isEntityAllowed = true;
                break;
            }
        }

        if (isEntityAllowed)
        {
            if(EntitiesInArea.Count < MaximumEntitiesAllowedInArea)
            {
                EntitiesInArea.Add(entity);
                return;
            } else
            {
                AddEntityToArea(entity);
            }
        }
    }
}