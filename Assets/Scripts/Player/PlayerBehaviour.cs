using System;
using UnityEngine;

public class PlayerBehaviour : BaseMovement
{
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;
    public HighlightBehaviour HighlightBehaviour;
    public LevitateBehaviour LevitateBehaviour;

    [SerializeField] private CameraController _cameraController;

    [SerializeField] private Vector3 input = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        HighlightBehaviour.HighlightGameobjectsInRadius();

        //Posses behaviour
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PossessionBehaviour.IsPossessing)
            {
                PossessionBehaviour.LeavePossessedTarget();
            } 
            else
            {
                PossessionBehaviour.PossessTarget();
            }
        }

        //Dash behaviour
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!DashBehaviour.IsDashing && !DashBehaviour.DashOnCooldown)
            {
                DashBehaviour.Dash();
            }
        }
        
        //levitatebehaviour
        
        HandleLevitationInput();
        
        //Move player with BaseMovement.
        if (!DashBehaviour.IsDashing && !PossessionBehaviour.IsPossessing)
        {
            Vector3 moveDirection = Input.GetAxis("Vertical") * _cameraController.transform.forward +
                                    Input.GetAxis("Horizontal") * _cameraController.transform.right;
            moveDirection.y = 0;
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                MoveEntityInDirection(moveDirection);
            else Rigidbody.velocity = Vector3.zero;
        }
        
    }

    private void FixedUpdate()
    {
        LevitateBehaviour.MoveLevitateableObject();
    }
    

    private void HandleLevitationInput()
    {
        LevitateBehaviour.FindObjectInFrontOfPLayer();
        
        if (Input.GetMouseButtonDown(0))
        {
            LevitateBehaviour.LevitationStateHandler();
        }
        
        if (Input.GetMouseButton(1))
        {
            RotationHandler(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            RotationHandler(false);
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) LevitateBehaviour.RemoveRigidbodyAndChangeState();

        LevitateBehaviour.PushOrPullLevitateableObject();
    }

    private void RotationHandler(bool isRotating)
    {
        LevitateBehaviour.IsRotating = isRotating;
        Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.None;
        LevitateBehaviour.RotateLevitateableObject();
    }
}
