using UnityEngine;

public class OpenPopupOnAwake : MonoBehaviour
{
    public Popup Popup;

    private void Awake()
    {
        if (Popup)
            Popup.OpenPopup();
    }
}
