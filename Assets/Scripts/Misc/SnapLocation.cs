using Enums;
using UnityEngine;

public class SnapLocation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnappableLevitationObject _snappableLevitationObject;
    
    [Header("Position & Rotation")]
    [SerializeField] private Transform _snapPosition;
    [SerializeField] private Vector3 _rotation;

    public SnapLocationState CurrentState { get; set; }

    private void Awake()
    {
        CurrentState = SnapLocationState.NotOccupied;
    }

    public bool IsSnappableObjectValid(SnappableLevitationObject snappableLevitationObjectParam)
    {
        if (!_snappableLevitationObject || !snappableLevitationObjectParam) return false;
        return _snappableLevitationObject.GetInstanceID() == snappableLevitationObjectParam.GetInstanceID();
    }

    public void SnapGameObject(SnappableLevitationObject snappableLevitationObject)
    {
        if (CurrentState == SnapLocationState.Occupied) return;
        
        if (IsSnappableObjectValid(snappableLevitationObject))
        {
            snappableLevitationObject.transform.position = new Vector3(
                _snapPosition.position.x,
                _snapPosition.position.y,
                _snapPosition.position.z
            );
            snappableLevitationObject.transform.eulerAngles = _rotation;

            Rigidbody snappableObjectRigidbody = _snappableLevitationObject.gameObject.GetComponent<Rigidbody>();
            snappableObjectRigidbody.isKinematic = true;
            snappableObjectRigidbody.useGravity = false;
            
            Destroy(snappableObjectRigidbody);
            CurrentState = SnapLocationState.Occupied;
        }
    }
}
