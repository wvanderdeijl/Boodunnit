using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class TerrifyBehaviour : MonoBehaviour
{
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _distance = 20f;

    private List<IFearable> _fearables;

    private void Awake()
    {
        _fearables = new List<IFearable>();
    }

    public void TerrifyEntities()
    { 
        GetEntitiesInProximity();
        
        foreach (IFearable fearable in _fearables)
        {
            fearable.Fear();
        }
    }

    private void GetEntitiesInProximity()
    {
        _fearables.Clear();
        
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, _radius, transform.forward,
            _distance);
        
        foreach (RaycastHit hit in hits)
        {
            IFearable fearable = hit.collider.GetComponent<IFearable>();
            if(fearable != null) _fearables.Add(fearable);
        }
    }
}
