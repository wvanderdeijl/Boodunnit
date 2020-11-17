using Interfaces;
using UnityEngine;

public class SnappableObject : MonoBehaviour, ISnappable
{
    [Header("Overlap Sphere")]
    [SerializeField] private float _overlapSphereRadius = 5f;

    private Collider GetClosestSnaplocation()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _overlapSphereRadius);

        Collider closestCollider = null;
        float currentClosestDistance = _overlapSphereRadius;
        
        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);

            if (distance < currentClosestDistance && hitCollider.GetComponent<SnapLocation>() != null)
            {
                Debug.Log(hitCollider.gameObject);
                
                currentClosestDistance = distance;
                closestCollider = hitCollider;
            }
        }
        
        return closestCollider;
    }
    
    public void Snap()
    {
        Collider collider = GetClosestSnaplocation();

        SnapLocation snapLocation = collider.GetComponent<SnapLocation>();

        if (snapLocation)
        {
            Rigidbody myRigidboyd = GetComponent<Rigidbody>();
            
            snapLocation.SnapGameObject(gameObject.GetComponent<SnappableObject>());
            myRigidboyd.isKinematic = true;
            myRigidboyd.useGravity = false;
        }
    }
}
