using System;
using UnityEngine;

public class PlayerBehaviour : BaseMovement
{
    public PauseMenu PauseMenu;
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;
    public HighlightBehaviour HighlightBehaviour;
    public LevitateBehaviour LevitateBehaviour;
    public DialogueManager DialogueManager;

    [Header("Player Interaction Radius")]
    public Transform InteractPoint;
    public float InteractRadius;

    private CameraController _cameraController;

    private void Awake()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        HighlightBehaviour.HighlightGameobjectsInRadius();

        //Pause game behaviour
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.TogglePauseGame();
        }

        //Return when the game is paused, so there can be no input buffer
        if (PauseMenu.IsPaused)
        {
            return;
        }

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

        //Dialogue behaviour
        if (Input.GetKey(KeyCode.F))
        {
            if (!DialogueManager.hasDialogueStarted)
            {
                DialogueManager.TriggerDialogue(InteractPoint, InteractRadius);
            }
        }

        //Dash behaviour
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!DashBehaviour.IsDashing && !DashBehaviour.DashOnCooldown)
            {
                DashBehaviour.Dash();
            }
        }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(InteractPoint.position, InteractRadius);
    }

    private void OnApplicationQuit()
    {
        PlayerData player = new PlayerData
        {
            PlayerPositionX = transform.position.x,
            PlayerPositionY = transform.position.y,
            PlayerPositionZ = transform.position.z,
            PlayerRotationX = transform.rotation.x,
            PlayerRotationY = transform.rotation.y,
            PlayerRotationZ = transform.rotation.z,
        };

        SaveHandler.Instance.SaveDataContainer(player);
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
        Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.Confined;
        LevitateBehaviour.RotateLevitateableObject();
    }
}
