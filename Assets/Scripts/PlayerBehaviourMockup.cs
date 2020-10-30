using UnityEngine;

public class PlayerBehaviourMockup : MonoBehaviour
{
    [SerializeField] private LevitateBehaviour _levitateBehaviour;
    
    private void Update()
    {
        _levitateBehaviour.FindObjectInFrontOfPLayer();
        
        if (Input.GetMouseButtonDown(0))
        {
            _levitateBehaviour.LevitationStateHandler();
        }
        
        if (Input.GetMouseButton(1))
        {
            _levitateBehaviour.RotateLevitateableObject();
        }

        _levitateBehaviour.PushOrPullLevitateableObject();
    }

    private void FixedUpdate()
    {
        _levitateBehaviour.MoveLevitateableObject();
    }
}
