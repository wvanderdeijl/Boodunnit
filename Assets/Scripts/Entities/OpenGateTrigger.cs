using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenGateTrigger : MonoBehaviour
{
    public GameObject RatEntityArea;

    private bool _hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasBeenTriggered && FindObjectOfType<PlayerBehaviour>() == other.GetComponent<PlayerBehaviour>())
        {
            MoveEntityArea();
        }
    }

    private void MoveEntityArea()
    {
        if (RatEntityArea)
        {
            RatEntityArea.transform.position = new Vector3(-6.96550226f, -40.6840744f, -32.8300018f);
        }
    }
}
