using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class PossessionBehaviour : MonoBehaviour
{
    public bool IsPossessing;
    public IEntity TargetBehaviour;

    public Image CooldownImage;

    public static GameObject PossessionTarget;
    public CameraController CameraController;
    private float _possessionRadius = 1;

    public float Cooldown = 1f;
    public bool IsOnCooldown = false;
    

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

            EnableOrDisablePlayerMeshRenderers(true);
            EnableOrDisablePlayerColliders(true);

            TargetBehaviour = null;
            PossessionTarget = null;
            
            IsOnCooldown = true;

            StartCoroutine(PossessionTimer());
        }
    }

    private void EnableOrDisablePlayerMeshRenderers(bool activateMeshRenderers)
    {
        //Changes players mesh renderer
        MeshRenderer playerMeshRenderer = GetComponent<MeshRenderer>();
        if (playerMeshRenderer)
        {
            playerMeshRenderer.enabled = activateMeshRenderers;
        }

        //Changes all of the players children mesh renderers
        MeshRenderer[] arrayPlayerChildMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childMeshRenderer in arrayPlayerChildMeshRenderers)
        {
            childMeshRenderer.enabled = activateMeshRenderers;
        }
    }

    private void EnableOrDisablePlayerColliders(bool activateColliders)
    {
        //Changes players collider
        Collider playerCollider = GetComponent<Collider>();
        if (playerCollider)
        {
            playerCollider.enabled = activateColliders;
        }

        //Changes all of the players children colliders
        Collider[] arrayPlayerChildColliders = GetComponentsInChildren<Collider>();
        foreach (Collider childCollider in arrayPlayerChildColliders)
        {
            childCollider.enabled = activateColliders;
        }
    }

    public void PossessTarget()
    {
        if (IsPossessing || IsOnCooldown)
        {
            return;
        }

        //It was really frustrating for everyone to possess something, so i the lead dev created this, this is code that will be used for now, later on interaction with the world will change (for example by hovering your mouse on an object and pressing E to possess). Dont worry about this new kind of input yet.
        float overlapSphereRadius = 3;
        List<Collider> listGameObjectsInRangeOrderedByRange = Physics.OverlapSphere(transform.position, overlapSphereRadius).OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToList();//ToDo: there is a possessable collision layer, should we use this layer or not? If we are using the layer use: LayerMask.GetMask() in the OverlapSphere method
        foreach (Collider gameObjectInRangeCollider in listGameObjectsInRangeOrderedByRange)
        {
            IPossessable possessableInterface = gameObjectInRangeCollider.GetComponent<IPossessable>();
            if (possessableInterface != null && !PossessionTarget)
            {
                TargetBehaviour = gameObjectInRangeCollider.GetComponent<IEntity>();
                PossessionTarget = gameObjectInRangeCollider.gameObject;
                CameraController.CameraRotationTarget = gameObjectInRangeCollider.transform;

                IsPossessing = true;
                transform.position = gameObjectInRangeCollider.gameObject.transform.position;

                EnableOrDisablePlayerMeshRenderers(false);
                EnableOrDisablePlayerColliders(false);

                //Stop iterating a possessable is found
                break;
            }
        }

        //ToDo: Did i do this right? i tried to mirror the code below as close as possible
        if (listGameObjectsInRangeOrderedByRange.Count == 0 && PossessionTarget)
        {
            PossessionTarget = null;
        }

        //if (Physics.SphereCast(transform.position, _possessionRadius, transform.forward, out RaycastHit raycastHit, 1))
        //{
        //    GameObject possessionGameObject = raycastHit.transform.gameObject;
        //    IPossessable possessableInterface = possessionGameObject.GetComponent<IPossessable>();
        //    if (possessionGameObject && possessableInterface != null && !PossessionTarget)
        //    {
        //        TargetBehaviour = possessionGameObject.GetComponent<IEntity>();
        //        PossessionTarget = possessionGameObject;
        //        CameraController.CameraRotationTarget = possessionGameObject.transform;

        //        IsPossessing = true;
        //        transform.position = PossessionTarget.transform.position;
        //        EnableOrDisablePlayerMeshRenderers(false);
        //        EnableOrDisablePlayerColliders(false);
        //    }
        //} 
        //else if (PossessionTarget)
        //{
        //    PossessionTarget = null;
        //}
    }

    private IEnumerator PossessionTimer()
    {
        float currentTime = 0;
        while (currentTime < Cooldown)
        {
            yield return null;
            currentTime += Time.deltaTime;
            CooldownImage.fillAmount = currentTime / Cooldown;
        }

        IsOnCooldown = false;
    }
}
