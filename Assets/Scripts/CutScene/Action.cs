using UnityEngine;

[System.Serializable]
public class Action
{
    public GameObject ObjectForCutscene;
    public Vector3 EndPosition;
    public Quaternion EndRotation;
    public Popup Popup;

    public ActionType ActionType;

    public int TransitionSpeed;
    public bool IsInstant;
    public bool IsExecuting;
    public float TimeBeforeNextAction;
}
