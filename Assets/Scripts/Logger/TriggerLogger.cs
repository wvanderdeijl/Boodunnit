using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TriggerLogger : MonoBehaviour
{
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
        
        Log log = new Log();
        log.LogTime = (float) (DateTime.UtcNow - _scenelogger.SceneLog.StartingTime).TotalSeconds;
        log.Name = gameObject.name;
        
        if (PossessionBehaviour.PossessionTarget)
            log.LogDetails.Add("Possessing: " + PossessionBehaviour.PossessionTarget.name);
        else log.LogDetails.Add("Possessing: None");
        if (log.LogDetails.Count > 0)
            _scenelogger.SceneLog.Logs.Add(log);
    }
}
