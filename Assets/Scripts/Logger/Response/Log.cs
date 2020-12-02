using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Log
{
    public string Name;
    /// <summary>
    /// 1st is the actual information
    /// 2nd Describes time in seconds after starting
    /// </summary>
    public List<object> LogDetails = new List<object>();
    public float LogTime;
}

