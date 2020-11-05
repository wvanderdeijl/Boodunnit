using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessableSanboxBox : BaseMovement, IPossessable
{
    private CameraController _cameraController;
    [SerializeField] private PossessionBehaviour _possessionBehaviour;
    private void Awake()
    {
        _cameraController = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (PossessionBehaviour.PossessionTarget == gameObject)
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
}
