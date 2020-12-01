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
    public List<CustomLogProperty> LogInformation = new List<CustomLogProperty>();
    public float LogTime;
}
public class CustomLogProperty
{
    public string Name;
    public object Value;

    public CustomLogProperty(string name, object value)
    {
        Name = name;
        Value = value;
    }
}
