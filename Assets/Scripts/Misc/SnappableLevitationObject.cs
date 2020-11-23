using UnityEngine;
using System.Linq;

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
            closestSnapLocation.SnapGameObject(snappableLevitationObject);
        }
    }
}
