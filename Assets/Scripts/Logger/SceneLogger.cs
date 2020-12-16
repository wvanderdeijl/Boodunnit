using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLogger : MonoBehaviour
{
    public SceneLog SceneLog = new SceneLog();
    public List<SceneInfoType> SceneInfo;

    private void Awake()
    {
        PlaythroughLogger.Instance.PlaythroughLog.GameStartTime = DateTime.UtcNow;
    }

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
        if (SceneInfo.Contains(SceneInfoType.All) || SceneInfo.Contains(SceneInfoType.LevitationsPerObject))
            SaveLevitationInfo();
        if (SceneInfo.Contains(SceneInfoType.All) || SceneInfo.Contains(SceneInfoType.PossessionsPerObject))
            SavePossessionInfo();
        
        PlaythroughLogger.Instance.PlaythroughLog.Scenes.Add(SceneLog);
        
    }

    private void SaveLevitationInfo()
    {
        Log log = new Log();
        log.Name = "Levitations";
        foreach (LevitateableObject levitateable in FindObjectsOfType<LevitateableObject>())
        {
            log.LogDetails.Add(new Log.Entry(
                new Log.Entry(levitateable.name, levitateable.TimesLevitated), 
                (DateTime.UtcNow - SceneLog.StartingTime).TotalSeconds)
            );
        }
        SceneLog.Stats.Add(log);
    }
    private void SavePossessionInfo()
    {
        Log log = new Log();
        log.Name = "Possesions";
        
        foreach (BaseEntity entity in FindObjectsOfType<BaseEntity>())
        {
            log.LogDetails.Add(new Log.Entry(
                new Log.Entry(entity.name, entity.TimesPosessed), 
                (DateTime.UtcNow - SceneLog.StartingTime).TotalSeconds)
            );
        }
        SceneLog.Stats.Add(log);
    }

    private void OnApplicationQuit()
    {
        PlaythroughLogger.Instance.PlaythroughLog.GameEndTime = DateTime.UtcNow;
        PlaythroughLogger.Instance.PlaythroughLog.Platform = Application.platform.ToString();
        PlaythroughLogger.Instance.WriteLog();
    }

}
