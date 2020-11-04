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
        LevitateBehaviour.FindObjectInFrontOfPLayer();
        
        if (Input.GetMouseButtonDown(0))
        {
            LevitateBehaviour.LevitationStateHandler();
        }
        
        if (Input.GetMouseButton(1))
        {
            LevitateBehaviour.RotateLevitateableObject();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt)) LevitateBehaviour.RemoveRigidbodyAndChangeState();

        LevitateBehaviour.PushOrPullLevitateableObject();
        
        //Move player with BaseMovement.
        if (!DashBehaviour.IsDashing && PossessionBehaviour.IsPossessing == false)
        {
            Vector3 moveDirection = Input.GetAxis("Vertical") * _cameraController.transform.forward +
                                    Input.GetAxis("Horizontal") * _cameraController.transform.right;
            moveDirection.y = 0;
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
                MoveEntityInDirection(moveDirection);
            else Rigidbody.velocity = Vector3.zero;

            _cameraController.RotateCamera(0);
        }
    }

    private void FixedUpdate()
    {
        LevitateBehaviour.MoveLevitateableObject();
    }
    
    
}
