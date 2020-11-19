using Enums;
using UnityEngine;

public class SnapLocation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnappableObject _snappableObject;
    
    [Header("Position & Rotation")]
    [SerializeField] private Transform _snapPosition;
    [SerializeField] private Quaternion _rotation;

    private SnapLocationState _currentState = SnapLocationState.NotOccupied;
    
    public bool IsSnapLocationAvailable { get; set; }

    private void DisableSnapLocation()
    {
        _currentState = SnapLocationState.Occupied;
    }

    public bool IsSnappableObjectValid(SnappableObject snappableObject)
    {
        bool isValid = _snappableObject.GetInstanceID() == snappableObject.GetInstanceID();

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

    public void SnapThisGameObject(SnappableObject snappableObject)
    {
        if (_currentState == SnapLocationState.Occupied) return;

        if (IsSnappableObjectValid(snappableObject))
        {
            snappableObject.transform.position = new Vector3(
                _snapPosition.position.x,
                _snapPosition.position.y,
                _snapPosition.position.z
            );
            
            snappableObject.transform.localRotation = _rotation;

            Rigidbody snappableObjectRigidbody = _snappableObject.gameObject.GetComponent<Rigidbody>();

            snappableObjectRigidbody.isKinematic = true;
            snappableObjectRigidbody.useGravity = false;
            
            Destroy(snappableObjectRigidbody);
            
            DisableSnapLocation();
        }
    }
}
