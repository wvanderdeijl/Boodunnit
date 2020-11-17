using Enums;
using UnityEngine;

public class SnapLocation : MonoBehaviour
{
    [Header("Snappeble Object")]
    [SerializeField] private SnappableObject _snappableObject;
    [SerializeField] private Transform _position;
    [SerializeField] private Quaternion _rotation;

    private SnapLocationState _currentState = SnapLocationState.NotOccupied;

    private void DisableSnapLocation()
    {
        _currentState = SnapLocationState.Occupied;
    }

    public void SnapGameObject(SnappableObject snappableObject)
    {
        if (snappableObject == _snappableObject)
        {
            //snappableObject.transform = _position;
            snappableObject.transform.localRotation = _rotation;
            DisableSnapLocation();
        }
    }
}
