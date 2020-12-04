using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Playthrough
{
    public string GUID;
    public string Platform;
    public DateTime GameStartTime = DateTime.UtcNow;
    public DateTime GameEndTime;
    public List<SceneLog> Scenes;
}
