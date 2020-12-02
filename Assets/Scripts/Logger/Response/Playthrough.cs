using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Playthrough
{
    public string GUID;
    public DateTime GameStartTime = DateTime.UtcNow;
    public List<SceneLog> Scenes;
}
