using UnityEngine;

public class PlayerBehaviourMockup : MonoBehaviour
{
    [SerializeField] private LevitateBehaviour _levitateBehaviour;

    private void Update()
    {
        _levitateBehaviour.ReceiveObjectRigidbodyWithMouseClick();
    }

    private void FixedUpdate()
    {
        _levitateBehaviour.MoveLevitateableObject();
    }
}
