using UnityEngine;

[System.Serializable]
public struct Action
{
    public GameObject ObjectForCutscene;
    public Vector3 StartPosition;
    public Quaternion StartRotation;
    public Vector3 EndPosition;
    public Quaternion EndRotation;

    public int TransitionSpeed;

    public Popup Popup;
    // Dialogue?

    public bool IsBlockingAction;
    public bool shouldUseCurrentObjectPosition;
}
