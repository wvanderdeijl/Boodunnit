using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;

public class TerrifyBehaviour : MonoBehaviour
{
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _angle = 90f;

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

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider collider in colliders)
        {
            Vector3 offset = (collider.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(offset, transform.forward);
            
            IFearable fearable = collider.GetComponent<IFearable>();

            if (dot * 100f >= (90 - (_angle / 2f)) && fearable != null) _fearables.Add(fearable);
        }
    }
}
