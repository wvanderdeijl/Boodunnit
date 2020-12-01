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
        //TODO do something with layers here LayerMask does not seem to work for this
        if (other.gameObject.name != "Player")
            return;
        Log log = new Log();
        log.LogTime = (float) (DateTime.UtcNow - _scenelogger.SceneLog.StartingTime).TotalSeconds;
        log.LogInformation.Add(new CustomLogProperty(gameObject.name + " Passed", true));
        log.Name = gameObject.name;
        
        _scenelogger.SceneLog.Logs.Add(log);
    }
}
