using Enums;
using UnityEngine;

public class SnapLocation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnappableLevitationObject _snappableLevitationObject;
    
    [Header("Position & Rotation")]
    [SerializeField] private Transform _snapPosition;
    [SerializeField] private Vector3 _rotation;

    private SnapLocationState _currentState = SnapLocationState.NotOccupied;
    
    public bool IsSnapLocationAvailable { get; set; }

    public bool IsSnappableObjectValid(SnappableLevitationObject snappableLevitationObjectParam)
    {
        if (!_snappableLevitationObject || !snappableLevitationObjectParam) return false;

        bool isValid = _snappableLevitationObject.GetInstanceID() == snappableLevitationObjectParam.GetInstanceID();

        switch (isValid)
        {
            case true:
                IsSnapLocationAvailable = true;
                break;
            
            case false:
                IsSnapLocationAvailable = false;
                break;
        }

        return isValid;
    }

    public void SnapGameObject(SnappableLevitationObject snappableLevitationObject)
    {
        if (_currentState == SnapLocationState.Occupied) return;
        
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
            _currentState = SnapLocationState.Occupied;
        }
    }
}
