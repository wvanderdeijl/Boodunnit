using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (FindObjectOfType<PlayerBehaviour>() == other.GetComponent<PlayerBehaviour>())
        {
            SceneTransitionHandler.Instance.GoToScene("CemetaryScene");
        }
    }
}
