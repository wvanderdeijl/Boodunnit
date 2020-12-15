using System.Linq;
using UnityEngine;

public class SnappableLevitationObject : MonoBehaviour
{
    [Header("Overlap Sphere")]
    [SerializeField] private float _overlapSphereRadius = 2f;
    
    public SnapLocation FindClosestValidSnaplocation()
    {
        SnappableLevitationObject snappableLevitationObject = GetComponent<SnappableLevitationObject>();
        
        Collider potentialSnapLocationCollider =  Physics
            .OverlapSphere(transform.position, _overlapSphereRadius)
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .Where(c =>
            {
                SnapLocation snapLocation = c.GetComponent<SnapLocation>();
                return snapLocation != null && snapLocation.IsSnappableObjectValid(snappableLevitationObject);
            })
            .FirstOrDefault();

        return potentialSnapLocationCollider ? potentialSnapLocationCollider.GetComponent<SnapLocation>() : null;
    }

    public void SnapThisObject()
    {
        SnapLocation closestSnapLocation = FindClosestValidSnaplocation();
        
        if (closestSnapLocation)
        {
            SnappableLevitationObject snappableLevitationObject = GetComponent<SnappableLevitationObject>();
            
            // changing layers (default layer changes back within the levitateable object script.
            int levitatingObjectLayerMask = LayerMask.NameToLayer("Default");
            foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>())
            {
                transform.gameObject.layer = levitatingObjectLayerMask;
            }     
            
            closestSnapLocation.SnapGameObject(snappableLevitationObject);
        }
    }
}