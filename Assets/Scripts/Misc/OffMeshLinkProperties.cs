using System;
using UnityEngine;

public class OffMeshLinkProperties : MonoBehaviour
{
    public float ParabolaHeight = 0;
    public float ParabolaDuration = 0;

    private void Awake()
    {
        if (ParabolaHeight == 0f) ParabolaHeight = 5.1f;
        if (ParabolaDuration == 0f) ParabolaDuration = 2.6f;
    }
}