using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TriggerLogger : MonoBehaviour
{
    public string Name;
    private SceneLogger _scenelogger;

    private void Awake()
    {
        _scenelogger = FindObjectOfType<SceneLogger>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        //TODO do something with layers here LayerMask does not seem to work for this
        if (other.gameObject.name != "Player")
            if (!(PossessionBehaviour.PossessionTarget &&
                  PossessionBehaviour.PossessionTarget.name == other.gameObject.name))
                return;
        Log last = new Log();
        if (_scenelogger.SceneLog.Logs.Count > 0)
        {
            last = _scenelogger.SceneLog.Logs[_scenelogger.SceneLog.Logs.Count - 1];
        }
        double logTime = (DateTime.UtcNow - _scenelogger.SceneLog.StartingTime).TotalSeconds;
        logTime = Math.Round(logTime, 3);
        if (last.Name == Name)
        {
            Log.Entry extraEntry = new Log.Entry();
            extraEntry.LogTime = (float) logTime;
            extraEntry.info = PossessionBehaviour.PossessionTarget
                ? "Possessing: " + PossessionBehaviour.PossessionTarget.name
                : "Possessing: None";
            last.LogDetails.Add(extraEntry);
            _scenelogger.SceneLog.Logs[_scenelogger.SceneLog.Logs.Count - 1] = last;
        }
        else
        {
            Log log = new Log();
            log.Name = Name;
            Log.Entry entry = new Log.Entry();
            entry.LogTime = (float) logTime;
            entry.info = PossessionBehaviour.PossessionTarget
                ? "Possessing: " + PossessionBehaviour.PossessionTarget.name
                : "Possessing: None";
            log.LogDetails.Add(entry);
        
            if (log.LogDetails.Count > 0)
                _scenelogger.SceneLog.Logs.Add(log);
        }
    }
}
