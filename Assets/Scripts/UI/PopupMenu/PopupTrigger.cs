using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTrigger : MonoBehaviour
{
    public Canvas Popup;

    public bool IsTriggerForCloud = false;

    public GameObject CloudSnapLocation;


    private bool _hasBeenTriggered = false;

    // Todo remove if when levitation behaviour is chosen
    private void OnTriggerEnter(Collider other)
    {
        if (Popup != null && !_hasBeenTriggered && FindObjectOfType<PlayerBehaviour>() == other.GetComponent<PlayerBehaviour>())
        {
            if (FindObjectOfType<PlayerBehaviourStaticLevitation>() ==
                other.GetComponent<PlayerBehaviourStaticLevitation>())
            {
                ActivateCloudOutline();

                GameManager.IsPaused = !GameManager.IsPaused;
                GameManager.CursorIsLocked = !GameManager.CursorIsLocked;
                Time.timeScale = GameManager.IsPaused ? 0 : 1;

                Popup.gameObject.SetActive(true);
                _hasBeenTriggered = true;
            }
        }
    }

    private void ActivateCloudOutline()
    {
        if (IsTriggerForCloud && CloudSnapLocation)
        {
            Outline outline = CloudSnapLocation.gameObject.GetComponent<Outline>();
            if (outline)
            {
                outline.enabled = true;
            }
        }
    }
}
