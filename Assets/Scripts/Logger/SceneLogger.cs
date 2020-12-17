using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLogger : MonoBehaviour
{
    public SceneLog SceneLog = new SceneLog();
    public List<SceneInfoType> SceneInfo;
    private List<BaseEntity> Possessables;
    private List<LevitateableObject> LevitateableObjects = new List<LevitateableObject>();
    private Log posessables = new Log();
    private Log levitatables = new Log();
    private List<Log> unusedLogs = new List<Log>();
    private TriggerLogger[] _triggerLoggers;

    private void Awake()
    {
        PlaythroughLogger.Instance.PlaythroughLog.GameStartTime = DateTime.UtcNow;
        _triggerLoggers = gameObject.GetComponentsInChildren<TriggerLogger>();
        Possessables = FindObjectsOfType<BaseEntity>().ToList();
        foreach (LevitateableObject obj in FindObjectsOfType<LevitateableObject>())
        {
            if (obj.WillLogPossessCount) LevitateableObjects.Add(obj);
        }
    }

    private void Start()
    {
        SceneLog.Name = SceneManager.GetActiveScene().name;
        SceneLog.StartingTime = DateTime.UtcNow;
    }

    private void Update()
    {
        posessables = SavePossessionInfo();
        levitatables = SaveLevitationInfo();
        unusedLogs = new List<Log>();
        foreach (TriggerLogger logger in _triggerLoggers)
        {
            if (!logger.hasLogged) unusedLogs.Add(new Log(logger.name));
        }
    }

    private void OnDestroy()
    {
        // if (SceneInfo.Contains(SceneInfoType.All) || SceneInfo.Contains(SceneInfoType.LevitationsPerObject))
        //     SaveLevitationInfo();
        // if (SceneInfo.Contains(SceneInfoType.All) || SceneInfo.Contains(SceneInfoType.PossessionsPerObject))
        //     SavePossessionInfo();
        // TriggerLogger[] logs = gameObject.GetComponentsInChildren<TriggerLogger>();
        // foreach (TriggerLogger logger in logs)
        // {
        //     if (!logger.hasLogged) SceneLog.UnusedLogs.Add(new Log(logger.name));
        // }
        //
        // PlaythroughLogger.Instance.PlaythroughLog.Scenes.Add(SceneLog);
        SceneLog.Stats.Add(posessables);
        SceneLog.Stats.Add(levitatables);
        SceneLog.UnusedLogs = unusedLogs;
        PlaythroughLogger.Instance.PlaythroughLog.Scenes.Add(SceneLog);
    }

    private Log SaveLevitationInfo()
    {
        Log log = new Log();
        log.Name = "Levitations";
        foreach (LevitateableObject levitateable in LevitateableObjects)
        {
            log.LogDetails.Add(
                new Log.Entry(levitateable.name, levitateable.TimesLevitated)
            );
        }

        return log;
    }
    private Log SavePossessionInfo()
    {
        Log log = new Log();
        log.Name = "Possesions";
        foreach (BaseEntity entity in Possessables)
        {
            log.LogDetails.Add(
                new Log.Entry(entity.name, entity.TimesPosessed)
            );
        }

        return log;
    }

    private void OnApplicationQuit()
    {
        PlaythroughLogger.Instance.PlaythroughLog.GameEndTime = DateTime.UtcNow;
        PlaythroughLogger.Instance.PlaythroughLog.Platform = Application.platform.ToString();
        PlaythroughLogger.Instance.WriteLog();
    }

    
}
