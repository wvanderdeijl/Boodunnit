using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockObjectDash : MonoBehaviour
{
    private Vector3 _bortDefaultLocation;
    private Vector3 _burtDefaultLocation;

    private void Awake()
    {
        GameObject bort = GameObject.Find("Bort");
        GameObject burt = GameObject.Find("Burt");
        if(bort && burt)
        {
            _bortDefaultLocation = bort.transform.position;
            _burtDefaultLocation = burt.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckIfBurtAndBortLeftDefaultLocation() || CheckIfBurtAndBortAreFainted())
        {
            gameObject.layer = LayerMask.NameToLayer("Dashable");
        } else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private bool CheckIfBurtAndBortLeftDefaultLocation()
    {
        GameObject bort = GameObject.Find("Bort");
        GameObject burt = GameObject.Find("Burt");

        if (bort && burt)
            return bort.transform.position != _bortDefaultLocation && burt.transform.position != _burtDefaultLocation;
        return false;
    }

    private bool CheckIfBurtAndBortAreFainted() 
    {
        BaseEntity bort = GameObject.Find("Bort").GetComponent<BaseEntity>();
        BaseEntity burt = GameObject.Find("Burt").GetComponent<BaseEntity>();
        if (bort && burt) 
            return bort.EmotionalState == Enums.EmotionalState.Fainted && burt.EmotionalState == Enums.EmotionalState.Fainted;
        return false;
    }
}
