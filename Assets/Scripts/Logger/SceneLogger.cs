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
        TriggerLogger[] logs = gameObject.GetComponentsInChildren<TriggerLogger>();
        foreach (TriggerLogger logger in logs)
        {
            if (!logger.hasLogged) SceneLog.UnusedLogs.Add(new Log(logger.name));
        }
        PlaythroughLogger.Instance.PlaythroughLog.Scenes.Add(SceneLog);
        PlaythroughLogger.Instance.WriteLog();
    }

    private void OnApplicationQuit()
    {
        PlaythroughLogger.Instance.PlaythroughLog.GameEndTime = DateTime.UtcNow;
        PlaythroughLogger.Instance.PlaythroughLog.Platform = Application.platform.ToString();
    }
}
