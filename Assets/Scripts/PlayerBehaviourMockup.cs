using UnityEngine;

public class PlayerBehaviourMockup : MonoBehaviour
{
    [SerializeField] private LevitateBehaviour _levitateBehaviour;
    
    private void Update()
    {
        HandleLevitationInput();
    }

    private void FixedUpdate()
    {
        _levitateBehaviour.MoveLevitateableObject();
    }

    private void HandleLevitationInput()
    {
        _levitateBehaviour.FindObjectInFrontOfPLayer();
        
        if (Input.GetMouseButtonDown(0))
        {
            _levitateBehaviour.LevitationStateHandler();
        }
        
        if (Input.GetMouseButton(1))
        {
            RotationHandler(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            RotationHandler(false);
        }

        _levitateBehaviour.PushOrPullLevitateableObject();
    }

    private void RotationHandler(bool isRotating)
    {
        _levitateBehaviour.IsRotating = isRotating;
        Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.None;
        _levitateBehaviour.RotateLevitateableObject();
    }
}
