using UnityEngine;
using System.Linq;

public class SnappableLevitationObject : MonoBehaviour
{
    [Header("Overlap Sphere")]
    [SerializeField] private float _overlapSphereRadius = 5f;

    public SnapLocation NearestSnapLocation { get; set; }

    private Collider FindClosestSnaplocation()
    {
        return Physics
            .OverlapSphere(transform.position, _overlapSphereRadius)
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .Where(c => c.GetComponent<SnapLocation>())
            .FirstOrDefault();
    }

    public void InstantiateNearestSnapLocation()
    {
        Collider collider = FindClosestSnaplocation();
        
        if (collider)
        {
            SnapLocation snapLocation = collider.GetComponent<SnapLocation>();
            NearestSnapLocation = snapLocation;
        }
    }

    public bool IsSnapLocationValid()
    {
        if (NearestSnapLocation)
        {
            SnappableLevitationObject snappableLevitationObject = GetComponent<SnappableLevitationObject>();
            return NearestSnapLocation.IsSnappableObjectValid(snappableLevitationObject);
        }

        return false;
    }

    public void Snap()
    {
        if (NearestSnapLocationExists())
        {
            SnappableLevitationObject snappableLevitationObject = GetComponent<SnappableLevitationObject>();
            NearestSnapLocation.SnapThisGameObject(snappableLevitationObject);
        }
    }

    private bool NearestSnapLocationExists()
    {
        return NearestSnapLocation;
    }
}
