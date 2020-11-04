using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessionBehaviour : MonoBehaviour
{
    public MeshRenderer PlayerMesh;
    public bool IsPossessing = false;

    public Image CooldownImage;

    private float _possessionRadius = 1;
    public GameObject PossessionTarget;
    public CameraController CameraController;

    private float _cooldown = 2f;
    private bool _isOnCooldown = false;
    
    //TODO replace this with a better routine
    private static PossessionBehaviour _instance;
    public static PossessionBehaviour Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void LeavePossessedTarget()
    {
        if (PossessionTarget && IsPossessing)
        {
            IsPossessing = false;
            transform.position = PossessionTarget.transform.position;
            CameraController.CameraRotationTarget = transform;
            PlayerMesh.enabled = true;
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
