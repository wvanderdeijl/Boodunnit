using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScreenResolution
{
    public int ScreenHeight;
    public int ScreenWidth;

    public ScreenResolution(int screenHeight, int screenWidth)
    {
        ScreenHeight = screenHeight;
        ScreenWidth = screenWidth;
    }
}
