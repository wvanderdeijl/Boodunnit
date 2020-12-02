using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLogger : MonoBehaviour
{
    public SceneLog SceneLog = new SceneLog();
    private void Start()
    {
        SceneLog.Name = SceneManager.GetActiveScene().name;
        SceneLog.StartingTime = DateTime.UtcNow;
    }
    
    private void OnDestroy()
    {
        PlaythroughLogger.Instance.PlaythroughLog.Scenes.Add(SceneLog);
        PlaythroughLogger.Instance.WriteLog();
    }
}
