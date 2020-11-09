using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class PossessionBehaviour : MonoBehaviour
{
    public MeshRenderer PlayerMesh;
    public bool IsPossessing = false;
    public IEntity TargetBehaviour;

    public Image CooldownImage;

    private float _possessionRadius = 1;
    public static GameObject PossessionTarget;
    public CameraController CameraController;

    private float _cooldown = 2f;
    private bool _isOnCooldown = false;
    

    private void Update()
    {
        if(IsPossessing) transform.position = PossessionTarget.transform.position;
    }

    public void LeavePossessedTarget()
    {
        if (PossessionTarget && IsPossessing)
        {
            IsPossessing = false;
            transform.position = PossessionTarget.transform.position;
            CameraController.CameraRotationTarget = transform;
            PlayerMesh.enabled = true;
            
            TargetBehaviour = null;
            PossessionTarget = null;
            
            _isOnCooldown = true;

            StartCoroutine(PossessionTimer());
        }
    }

    public void PossessTarget()
    {
        if (IsPossessing || _isOnCooldown)
        {
            return;
        }

        if (Physics.SphereCast(transform.position, _possessionRadius, transform.forward, out RaycastHit raycastHit, 1))
        {
            GameObject possessionGameObject = raycastHit.transform.gameObject;
            IPossessable possessableInterface = possessionGameObject.GetComponent<IPossessable>();
            if (possessionGameObject && possessableInterface != null && !PossessionTarget)
            {
                TargetBehaviour = possessionGameObject.GetComponent<IEntity>();
                PossessionTarget = possessionGameObject;
                CameraController.CameraRotationTarget = possessionGameObject.transform;
                
                IsPossessing = true;
                transform.position = PossessionTarget.transform.position;
                PlayerMesh.enabled = false;
            }
        } 
        else if (PossessionTarget)
        {
            PossessionTarget = null;
        }
    }

    private IEnumerator PossessionTimer()
    {
        float currentTime = 0;

        while (currentTime < _cooldown)
        {
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
            CooldownImage.fillAmount = currentTime / _cooldown;
        }
        _isOnCooldown = false;
    }
}
