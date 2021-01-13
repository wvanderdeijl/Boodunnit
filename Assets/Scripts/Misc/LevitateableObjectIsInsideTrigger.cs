using UnityEngine;

public class LevitateableObjectIsInsideTrigger : MonoBehaviour
{
    public bool PlayerIsInsideObject { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerBehaviour>())
        {
            PlayerIsInsideObject = true;
        }     
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerBehaviour>())
        {
            PlayerIsInsideObject = false;
        }
    }
}
