using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirBonkelSpawnLocationBehaviour : MonoBehaviour
{
    public Transform Spawnpoint;

    private SirBonkelBehaviour _sirBonkelBehaviour;

    private void Awake()
    {
        _sirBonkelBehaviour = FindObjectOfType<SirBonkelBehaviour>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBehaviour>() && _sirBonkelBehaviour)
        {
            _sirBonkelBehaviour.SpawnToNewLocation(Spawnpoint);
        }
    }
}
