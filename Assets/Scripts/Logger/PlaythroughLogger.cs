﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class PlaythroughLogger
{
    public Playthrough PlaythroughLog = new Playthrough();
    private static PlaythroughLogger _instance;
    public static PlaythroughLogger Instance { get => _instance; }

    static PlaythroughLogger() => _instance = new PlaythroughLogger();

    private PlaythroughLogger() {
        PlaythroughLog.Scenes = new List<SceneLog>();
        PlaythroughLog.GameEndTime = DateTime.UtcNow;
        PlaythroughLog.GUID = GUID.Generate().ToString();
    }
    
    public void WriteLog()
    {
        FirebaseHTTPController.PostPlaythrough(PlaythroughLog);
        // File.WriteAllText(Application.persistentDataPath + "/" + PlaythroughLog.GUID + ".json",
        //     JsonConvert.SerializeObject(PlaythroughLog));
    }
}