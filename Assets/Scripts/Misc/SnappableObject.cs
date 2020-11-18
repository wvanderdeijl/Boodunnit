using Interfaces;
using UnityEngine;

public class SnappableObject : MonoBehaviour, ISnappable
{
    [Header("Overlap Sphere")]
    [SerializeField] private float _overlapSphereRadius = 5f;

    public SnapLocation NearestSnapLocation { get; set; }

    private Collider FindClosestSnaplocation()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _overlapSphereRadius);
        Collider closestCollider = null;
        float currentClosestDistance = _overlapSphereRadius;
        
        foreach (Collider hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            SnapLocation snapLocation = hitCollider.GetComponent<SnapLocation>();

            if (distance < currentClosestDistance && snapLocation != null)
            {
                currentClosestDistance = distance;
                closestCollider = hitCollider;
            }
        }
        
        return closestCollider;
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
            SnappableObject snappableObject = GetComponent<SnappableObject>();
            return NearestSnapLocation.IsSnappableObjectValid(snappableObject);
        }

        return false;
    }

    public void Snap()
    {
        if (NearestSnapLocationExists())
        {
            SnappableObject snappableObject = GetComponent<SnappableObject>();
            NearestSnapLocation.SnapThisGameObject(snappableObject);
        }
    }

    private bool NearestSnapLocationExists()
    {
        return NearestSnapLocation;
    }
}
