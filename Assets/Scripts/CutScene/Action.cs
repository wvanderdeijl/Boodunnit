using UnityEngine;

[System.Serializable]
public class Action
{
    public ActionType ActionType;
    public GameObject ObjectForCutscene;
    public Vector3 EndPosition;
    public Vector3 EndRotation;
    public Vector3 EndScale;
    public int TransitionSpeed;
 
    public bool IsInstant;
    [HideInInspector] public bool IsExecuting;
    public float TimeBeforeNextAction;
    
    public Popup Popup;
    public MethodToCallEvent MethodToCallFromScript;
}

[System.Serializable]
public class MethodToCallEvent : UnityEngine.Events.UnityEvent
{
}
