﻿using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;

    // Update is called once per frame
    void Update()
    {
        //Posses behaviour
        if (Input.GetKey(KeyCode.E))
        {
            if (PossessionBehaviour.IsPossessing)
            {
                PossessionBehaviour.LeavePossessedTarget();
            } 
            else
            {
                PossessionBehaviour.PossessTarget();
            }
        }

        //Dash behaviour
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!DashBehaviour.IsDashing && !DashBehaviour.DashOnCooldown)
            {
                DashBehaviour.Dash();
            }
        }

        //Levitate behaviour
        if (Input.GetMouseButtonDown(0))
        {
            print("Key H was hit");
        }
    }
}
