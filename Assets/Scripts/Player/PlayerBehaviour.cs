using System;
using Interfaces;
using UnityEngine;

public class PlayerBehaviour : BaseMovement
{
    public PauseMenu PauseMenu;
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;
    public HighlightBehaviour HighlightBehaviour;
    public DialogueManager DialogueManager;

    [Header("Player Interaction Radius")]
    public Transform InteractPoint;
    public float InteractRadius;

    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
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

        //Levitate behaviour
        if (Input.GetMouseButtonDown(0))
        {
            print("Key H was hit");
        }
        
        //Move player with BaseMovement.
        Vector3 moveDirection = Input.GetAxis("Vertical") * _cameraTransform.forward +
                                Input.GetAxis("Horizontal") * _cameraTransform.right;
        moveDirection.y = 0;

        if (!DashBehaviour.IsDashing && !PossessionBehaviour.IsPossessing)
        {
            MoveEntityInDirection(moveDirection);   
        } 
        else if (PossessionBehaviour.IsPossessing)
        {
            PossessionBehaviour.TargetBehaviour.Move(moveDirection);
            
        }

        //Use first ability.
        if (PossessionBehaviour.IsPossessing && Input.GetKeyDown(KeyCode.K))
        {
            PossessionBehaviour.TargetBehaviour.UseFirstAbility();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(InteractPoint.position, InteractRadius);
    }
}
