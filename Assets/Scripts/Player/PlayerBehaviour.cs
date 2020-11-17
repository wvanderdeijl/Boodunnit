using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerBehaviour : BaseMovement
{
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;
    public HighlightBehaviour HighlightBehaviour;
    public LevitateBehaviour LevitateBehaviour;

    public DialogueManager DialogueManager;

    public PauseMenu PauseMenu;

    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    //This method is used for now, the way of picking up clues has to be thought of still. For now we use this
    private void PickupClueInRange()
    {
        float clueDetectionRadius = 4;
        List<Collider> listGameObjectsInRangeOrderedByRange = Physics.OverlapSphere(transform.position, clueDetectionRadius).OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToList();
        foreach (Collider collider in listGameObjectsInRangeOrderedByRange)
        {
            WorldSpaceClue worldSpaceClue = collider.GetComponent<WorldSpaceClue>();
            if (worldSpaceClue)
            {
                worldSpaceClue.AddToInventory();
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveHandler.Instance.DeleteSaveGame();
        }

        HighlightBehaviour.HighlightGameobjectsInRadius();

        //Pause game behaviour
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.TogglePauseGame();
        }
        
        //Return when the game is paused, so there can be no input buffer
        if (GameManager.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PickupClueInRange();
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
                DialogueManager.TriggerDialogue();
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

        Vector3 moveDirection = Input.GetAxisRaw("Vertical") * _cameraTransform.forward +
                                Input.GetAxisRaw("Horizontal") * _cameraTransform.right;
        moveDirection.y = 0;

        if (!DashBehaviour.IsDashing && !PossessionBehaviour.IsPossessing && !DialogueManager.hasDialogueStarted)
        {
            MoveEntityInDirection(moveDirection);   
        } 
        else if (PossessionBehaviour.IsPossessing)
        {
            PossessionBehaviour.TargetBehaviour.Move(moveDirection);
            
        }

        //Use first ability.
        if (PossessionBehaviour.IsPossessing && Input.GetKeyDown(KeyCode.Q))
        {
            PossessionBehaviour.TargetBehaviour.UseFirstAbility();
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            if (PossessionBehaviour.IsPossessing)
            {
                PossessionBehaviour.TargetBehaviour.EntityJump();
                return;
            }
            Jump();
        }

        CheckIfGrounded();
    }
    private void FixedUpdate()
    {
        LevitateBehaviour.MoveLevitateableObject();
    }

    private void OnApplicationQuit()
    {
        PlayerData playerDataContainer = new PlayerData
        {
            PlayerPositionX = transform.position.x,
            PlayerPositionY = transform.position.y,
            PlayerPositionZ = transform.position.z,

            PlayerRotationX = transform.rotation.x,
            PlayerRotationY = transform.rotation.y,
            PlayerRotationZ = transform.rotation.z,
        };

        SaveHandler.Instance.SaveDataContainer(playerDataContainer);
    }
    private void HandleLevitationInput()
    {
        LevitateBehaviour.FindObjectInFrontOfPLayer();//ToDo: This throws errors when a gameobject is destroy while in range
        
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

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            LevitateBehaviour.RemoveRigidbodyAndChangeState();
        }

        LevitateBehaviour.PushOrPullLevitateableObject();
    }

    private void RotationHandler(bool isRotating)
    {
        LevitateBehaviour.IsRotating = isRotating;
        GameManager.CursorIsLocked = isRotating;

        LevitateBehaviour.RotateLevitateableObject();
    }
}
