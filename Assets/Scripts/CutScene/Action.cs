﻿using UnityEngine;

[System.Serializable]
public class Action
{
    [Header("Which type of action should be performed?")]
    public ActionType ActionType;

    [Header("Does action require a gameobject?")]
    public GameObject ObjectForCutscene;

    [Header("Fill in if action is 'Position'")]
    public Vector3 EndPosition;
    
    [Header("Fill in if action is 'Rotation'")]
    public Vector3 EndRotation;

    [Header("Fill in if action is 'Scaling'")]
    public Vector3 EndScale;

    [Header("Value that the determines the speed of position, rotation and scaling.")]
    public int TransitionSpeed;
 
    [Header("Check this if you want the position, rotation or scaling to be changed instantly.")]
    public bool IsInstant;

    [HideInInspector] public bool IsExecuting;

    [Header("Time to wait before the next action is executed.")]
    public float TimeBeforeNextAction;
    
    [Header("Pass a reference to a popup if the action is 'PopUp'")]
    public Popup Popup;

    [Header("Click the '+' button and pass a reference to a scrip if the action is 'Method'")]
    public MethodToCallEvent MethodToCallFromScript;
}

[System.Serializable]
public class MethodToCallEvent : UnityEngine.Events.UnityEvent
{
}
