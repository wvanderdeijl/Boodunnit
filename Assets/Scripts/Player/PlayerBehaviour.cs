﻿using UnityEngine;

public class PlayerBehaviour : BaseMovement
{
    public PossessionBehaviour PossessionBehaviour;
    public DashBehaviour DashBehaviour;
    public HighlightBehaviour HighlightBehaviour;
    public LevitateBehaviour LevitateBehaviour;

    public ConversationManager ConversationManager;

    public PauseMenu PauseMenu;

    private Transform _cameraTransform;
    private int _dashCounter;


    [HideInInspector]
    public Animator Animator;

    private void Awake()
    {
        _cameraTransform = UnityEngine.Camera.main.transform;
        CanJump = true;

        Animator = GetComponent<Animator>();
    }

    void Update()
    {

        HighlightBehaviour.HighlightGameobjectsInRadius();

        //Pause game behaviour
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.ToggleCursor();
            PauseMenu.TogglePauseGame();
        }

        //Return when the game is paused, so there can be no input buffer
        if (GameManager.IsPaused)
        {
            return;
        }

        //Posses behaviour
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PossessionBehaviour.IsPossessing && !ConversationManager.HasConversationStarted)
            {
                PossessionBehaviour.LeavePossessedTarget();
            }
            else
            {
                if (!DashBehaviour.IsDashing && !ConversationManager.HasConversationStarted && !LevitateBehaviour.IsLevitating)
                    PossessionBehaviour.PossessTarget();
            }
        }

        //Dialogue behaviour
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!ConversationManager.HasConversationStarted && !DashBehaviour.IsDashing && !LevitateBehaviour.IsLevitating)
            {
                ConversationManager.TriggerConversation(PossessionBehaviour.IsPossessing);
            }
        }

        //Dash behaviour
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!DashBehaviour.IsDashing && !DashBehaviour.DashOnCooldown && !ConversationManager.HasConversationStarted)
            {
                // If player is grounded he can always dash.
                if (IsGrounded)
                    DashBehaviour.Dash();

                // If player is not grounded, we check _dashCounter.
                if (!IsGrounded && _dashCounter <= 0)
                {
                    DashBehaviour.Dash();
                    _dashCounter++;
                }
            }
        }

        if (!PossessionBehaviour.IsPossessing && !ConversationManager.HasConversationStarted)
        {
            HandleLevitationInput();
        }

        //Move player with BaseMovement.
        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movementInput.x != 0 && movementInput.y != 0)
        {
            movementInput.x *= Mathf.Sqrt(2) / 2;
            movementInput.y *= Mathf.Sqrt(2) / 2;
        }

        Vector3 moveDirection = movementInput.y * _cameraTransform.forward +
                                movementInput.x * _cameraTransform.right;
        moveDirection.y = 0;

        if (!DashBehaviour.IsDashing && !PossessionBehaviour.IsPossessing && !ConversationManager.HasConversationStarted)
        {
            MoveEntityInDirection(moveDirection);
        }
        else if (PossessionBehaviour.IsPossessing)
        {
            PossessionBehaviour.TargetBehaviour.MoveEntityInDirection(moveDirection);
        }

        //Use first ability.
        if (PossessionBehaviour.IsPossessing && Input.GetKeyDown(KeyCode.Q))
        {
            PossessionBehaviour.TargetBehaviour.UseFirstAbility();
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && !ConversationManager.HasConversationStarted)
        {
            if (PossessionBehaviour.IsPossessing)
            {
                PossessionBehaviour.TargetBehaviour.Jump();
                return;
            }

            Jump();
        }

        if (IsGrounded)
        {
            // Reset dash counter for single in air dash.
            _dashCounter = 0;
        }
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
        LevitateBehaviour.FindLevitateableObjectsInFrontOfPlayer();

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
            LevitateBehaviour.RemoveRigidbodyAndStartFreeze();
        }

        LevitateBehaviour.PushOrPullLevitateableObject();
    }

    private void RotationHandler(bool isRotating)
    {
        LevitateBehaviour.IsRotating = isRotating;
        GameManager.CursorIsLocked = isRotating;

        LevitateBehaviour.RotateLevitateableObject();
    }

    private void PlayerMoveAnimation()
    {
        if (Rigidbody.velocity.magnitude > 0.01)
        {
            if (Animator)
                Animator.SetBool("IsMoving", true);
        }
        else
        {
            if (Animator)
                Animator.SetBool("IsMoving", false);
        }
    }
    private void PlayerDashAnimation()
    {
        if (DashBehaviour.IsDashing)
        {
            if (Animator)
                Animator.SetBool("IsDashing", true);
        }
        else
        {
            if (Animator)
                Animator.SetBool("IsDashing", false);
        }
    }
    private void PlayerJumpAnimation()
    {
        if (IsGrounded)
        {
            if (Animator)
                Animator.SetBool("IsGrounded", true);
        }
        else
        {
            if (Animator)
                Animator.SetBool("IsGrounded", false);
        }
    }
    private void PlayerLevitateAnimation()
    {
        if (LevitateBehaviour.IsLevitating)
        {
            if (Animator)
                Animator.SetBool("IsLevitating", true);
        }
        else
        {
            if (Animator)
                Animator.SetBool("IsLevitating", false);
        }
    }
}
