using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PossessionBehaviour : MonoBehaviour
{
    public bool IsPossessing;
    public IEntity TargetBehaviour;

    public Image CooldownImage;

    public static GameObject PossessionTarget;
    public CameraController CameraController;
    public float UnpossessRadius;
    public int UnPossessRetriesOnYAxis;

    public float Cooldown = 1f;
    public bool IsOnCooldown = false;

    private float _playerEndPositionRadius;

    private void Awake()
    {
        _playerEndPositionRadius = GetComponent<Collider>().bounds.extents.z;
    }

    private void Update()
    {
        if(IsPossessing) transform.position = PossessionTarget.transform.position;
    }

    public void LeavePossessedTarget()
    {
        if (PossessionTarget && IsPossessing)
        {
            IsPossessing = false;
            transform.position = PossessionTarget.transform.position + (Vector3.up * 2);
            CameraController.CameraRotationTarget = transform;

            EnableOrDisablePlayerMeshRenderers(true);
            EnableOrDisablePlayerColliders(true);

            TargetBehaviour.IsPossessed = false;
            PossessionTarget.GetComponent<NavMeshAgent>().enabled = true;
            PossessionTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            TargetBehaviour = null;
            PossessionTarget = null;
            
            IsOnCooldown = true;

            TeleportPlayerToRandomPosition();

            //ToDo: This code is left by the lead dev, change this to how it should be!, but right now i dont care
            GetComponent<Rigidbody>().velocity = Vector3.zero;

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

                PossessionTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                PossessionTarget.GetComponent<NavMeshAgent>().enabled = false;
                TargetBehaviour.IsPossessed = true;
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
    }

    private void TeleportPlayerToRandomPosition()
    {
        /**
            * 1. Create a random point for the player to teleport to.
            * 2. Check if this point is valid, does the player collide with any object?
            * 3. If point found is invalid, retry.
            * 4. Update player position to random position in radius.
        **/

        float minNewPlayerPositionInRadiusX = transform.position.x - UnpossessRadius;
        float minNewPlayerPositionInRadiusZ = transform.position.z - UnpossessRadius;

        float maxNewPlayerPositionInRadiusX = transform.position.x + UnpossessRadius;
        float maxNewPlayerPositionInRadiusY = transform.position.y + UnpossessRadius;
        float maxNewPlayerPositionInRadiusZ = transform.position.z + UnpossessRadius;

        bool isPositionValid = false;
        int newPositionTries = 0;

        while (!isPositionValid || newPositionTries == UnPossessRetriesOnYAxis)
        {
            Vector3 playerNewPositionAfterUnpossessing = GetNewPlayerVector3Position(minNewPlayerPositionInRadiusX, maxNewPlayerPositionInRadiusX, transform.position.y,
                minNewPlayerPositionInRadiusZ, maxNewPlayerPositionInRadiusZ);

            Collider[] newPositionCollision = Physics.OverlapSphere(playerNewPositionAfterUnpossessing, _playerEndPositionRadius);
            if (newPositionCollision != null)
            {
                if (newPositionCollision.Length == 0)
                {
                    transform.position = playerNewPositionAfterUnpossessing;
                    isPositionValid = true;
                }
            }

            newPositionTries++;
        }

        if(newPositionTries >= UnPossessRetriesOnYAxis)
        {
            Vector3 playerNewPositionAfterUnpossessing = GetNewPlayerVector3Position(minNewPlayerPositionInRadiusX, maxNewPlayerPositionInRadiusX, transform.position.y, maxNewPlayerPositionInRadiusY,
                minNewPlayerPositionInRadiusZ, maxNewPlayerPositionInRadiusZ);
            transform.position = playerNewPositionAfterUnpossessing;
        }
    }

    private Vector3 GetNewPlayerVector3Position(float minPositionX, float maxPositionX, float minPositionY, float maxPositionY, float minPositionZ, float maxPositionZ)
    {
        return new Vector3(
            Random.Range(minPositionX, maxPositionX),
            Random.Range(minPositionY, maxPositionY),
            Random.Range(minPositionZ, maxPositionZ)
        );
    }

    private Vector3 GetNewPlayerVector3Position(float minPositionX, float maxPositionX, float positionY, float minPositionZ, float maxPositionZ)
    {
        return new Vector3(
            Random.Range(minPositionX, maxPositionX),
            positionY,
            Random.Range(minPositionZ, maxPositionZ)
        );
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, UnpossessRadius);
    }
}
