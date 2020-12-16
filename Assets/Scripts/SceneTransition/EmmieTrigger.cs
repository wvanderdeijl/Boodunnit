using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmmieTrigger : MonoBehaviour
{
    public static bool HasVisitedEmmie;

    private void OnTriggerEnter(Collider other)
    {
        HasVisitedEmmie = true;
    }
}
