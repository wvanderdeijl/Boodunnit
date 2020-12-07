using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    public Canvas Popup;

    private bool _hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (Popup != null && !_hasBeenTriggered && FindObjectOfType<PlayerBehaviour>() == other.GetComponent<PlayerBehaviour>())
        {
            GameManager.IsPaused = !GameManager.IsPaused;
            GameManager.CursorIsLocked = !GameManager.CursorIsLocked;
            Time.timeScale = GameManager.IsPaused ? 0 : 1;

            Popup.gameObject.SetActive(true);
            _hasBeenTriggered = true;
        }
    }
}
