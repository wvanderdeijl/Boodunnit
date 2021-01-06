using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCredits : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("End"))
        {
            print("Done");
        }
    }
}
