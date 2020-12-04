using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Log
{
    public string Name;
    public List<Entry> LogDetails = new List<Entry>();
    /// <summary>
    /// Describes time in seconds after starting
    /// </summary>

    public class Entry
    {
        public object info;
        public float LogTime;
    }

}

