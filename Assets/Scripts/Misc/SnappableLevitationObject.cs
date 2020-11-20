using UnityEngine;
using System.Linq;

public class SnappableLevitationObject : MonoBehaviour
{
    [Header("Overlap Sphere")]
    [SerializeField] private float _overlapSphereRadius = 2f;
    public SnapLocation NearestSnapLocation { get; set; }

    private Collider FindClosestValidSnaplocation()
    {
        return Physics
            .OverlapSphere(transform.position, _overlapSphereRadius)
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .Where(c =>
            {
                SnappableLevitationObject snappableLevitationObject = GetComponent<SnappableLevitationObject>();
                SnapLocation snapLocation = c.GetComponent<SnapLocation>();
                return snapLocation != null && snapLocation.IsSnappableObjectValid(snappableLevitationObject);
            })
            .FirstOrDefault();
    }

    public void InstantiateNearestValidSnapLocation()
    {
        Collider collider = FindClosestValidSnaplocation();
        
        if (collider)
        {
            SnapLocation snapLocation = collider.GetComponent<SnapLocation>();
            NearestSnapLocation = snapLocation;
        }
    }

    public void Snap()
    {
        if (NearestSnapLocation)
        {
            SnappableLevitationObject snappableLevitationObject = GetComponent<SnappableLevitationObject>();
            NearestSnapLocation.SnapGameObject(snappableLevitationObject);
        }
    }
}
