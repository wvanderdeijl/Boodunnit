using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPopupOnAwake : MonoBehaviour
{
    Popup Popup;

    private void Awake()
    {
        if (Popup)
            Popup.OpenPopup();
    }
}
