using System;
using DefaultNamespace.Enums;
using UnityEngine;
using System.Linq;

public class LevitateBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;

    [Header("OverlapSphere")]
    [SerializeField] private float _overlapSphereRadiusInUnits = 5f;
    [SerializeField][Range(0, 360)] private float _overlapSphereAngleInDegrees = 360f;
    
    [Header("Speeds")]
    [SerializeField]private float _velocitySpeedPercentage = 25f;
    [SerializeField] private float _pushPullSpeed = 300f;
    
    [Header("Durations")]
    [SerializeField] private float _frozenDurationInSeconds = 5f;
    
    [Header("Distances")]
    [SerializeField] private float _minimumSelectionDistanceInUnits = 2f;
    
    public bool IsLevitating { get; set; }
    public Collider[] CurrentLevitateableObjects { get; set; }

    private Rigidbody _selectedRigidbody;
    private float _selectionDistance;
    
    private Vector3 _originalScreenTargetPosition;
    private Vector3 _originalRigidbodyPosition;
    
    public void LevitationStateHandler()
    {
        if (!_selectedRigidbody)
        {
            GetRigidbodyAndStartLevitation();
            if (_selectedRigidbody) _selectedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        else
        {
            _selectedRigidbody.constraints = RigidbodyConstraints.None;
            RemoveRigidbodyAndStartFreeze();
        }
    }

    public void MoveLevitateableObject()
    {
        if (!_selectedRigidbody) return;
        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();
        Collider collider = _selectedRigidbody.GetComponent<Collider>();

        if (!levitateable.CanBeLevitated || !IsObjectInLevitateablesArray(collider))
        {
            RemoveGameObjectFromCursor();
            return;
        }
        
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _selectionDistance);
        Vector3 mousePositionOffset = _mainCamera.ScreenToWorldPoint(mousePosition) - _originalScreenTargetPosition;
            
        _selectedRigidbody.velocity = 
            (_originalRigidbodyPosition + mousePositionOffset - _selectedRigidbody.transform.position) 
            * (500 * Time.deltaTime * (_velocitySpeedPercentage / 100));
    }

    public void PushOrPullLevitateableObject()
    {
        if (!_selectedRigidbody) return;
        
        if (_selectionDistance < _minimumSelectionDistanceInUnits)
        {
            _selectionDistance = _minimumSelectionDistanceInUnits + 0.1f;
            return;
        }
            
        _selectionDistance += (Input.GetAxis("Mouse ScrollWheel") * _pushPullSpeed * Time.deltaTime);
    }

    private void GetRigidbodyAndStartLevitation()
    {
        _selectedRigidbody = GetRigidbodyFromMouseClick();
        if (!_selectedRigidbody) return;
        IsLevitating = true;
        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();
        levitateable.TimesLevitated += 1;
        if (levitateable != null) levitateable.State = LevitationState.Levitating;
    }

    public void RemoveRigidbodyAndStartFreeze()
    {
        ILevitateable levitateable =
            _selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<ILevitateable>() : null;
        
        if (levitateable != null)
        {

            IsLevitating = false;
            levitateable.State = LevitationState.Frozen;
            SnappableLevitationObject snappableLevitationObject = 
                _selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<SnappableLevitationObject>() : null;

            if (snappableLevitationObject)
            {
                SnapLocation validSnapLocation = snappableLevitationObject.FindClosestValidSnaplocation();

                if (validSnapLocation)
                {
                    snappableLevitationObject.SnapThisObject();
                    _selectedRigidbody = null;
                    levitateable.State = LevitationState.SnappedIntoPlace;
                    return;
                }
            }
        }

        ActivateLevitateCoRoutine();
        RemoveSelectedRigidbody();
    }

    private Rigidbody GetRigidbodyFromMouseClick()
    {
        if (_selectedRigidbody)
        {
            ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();
            Collider collider = _selectedRigidbody.GetComponent<Collider>();

            if (levitateable != null && 
                (!levitateable.CanBeLevitated
                || !IsObjectInLevitateablesArray(collider) || levitateable.State == LevitationState.Frozen))
            {
                return null;
            }
        }
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Rigidbody rigidbody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            
            if (!rigidbody) return null;

            if (!hitInfo.collider.gameObject.GetComponent(typeof(ILevitateable))) return null;

            _selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
            _originalScreenTargetPosition = _mainCamera.ScreenToWorldPoint(
                new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    _selectionDistance
                )
            );

            _originalRigidbodyPosition = hitInfo.collider.transform.position;

            return rigidbody;
        }

        return null;
    }

    private void ActivateLevitateCoRoutine()
    {
        ILevitateable levitateable =
            _selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<ILevitateable>() : null;   
        
        if (_selectedRigidbody && levitateable != null)
        {
            StartCoroutine(levitateable.LevitateForSeconds(_frozenDurationInSeconds));
        }
    }

    public void FindLevitateableObjectsInFrontOfPlayer()
    {
        CurrentLevitateableObjects = Physics
            .OverlapSphere(transform.position, _overlapSphereRadiusInUnits)
            .Where(c => { return IsObjectInRange(c) && IsObjectLevitateble(c) && IsObjectInAngle(c); })
            .ToArray();
    }
    
    private bool IsObjectInRange(Collider collider)
    {
        return Vector3.Distance(transform.position, collider.transform.position) <= _overlapSphereRadiusInUnits;
    }
    
    private bool IsObjectLevitateble(Collider collider)
    {
        ILevitateable levitateableObject = collider.GetComponent<ILevitateable>();
        return levitateableObject != null;
    }
    
    private bool IsObjectInAngle(Collider collider)
    {
        Vector3 colliderDirection = collider.transform.position - transform.position;
        float angle = Vector3.Angle(colliderDirection, _player.transform.forward);
        return angle > -(_overlapSphereAngleInDegrees / 2) && angle < _overlapSphereAngleInDegrees / 2;
    }

    private bool IsObjectInLevitateablesArray(Collider collider)
    {
        if (CurrentLevitateableObjects.Length > 0)
        {
            return CurrentLevitateableObjects.Contains(collider);
        }

        return false;
    }
    
    private void RemoveGameObjectFromCursor()
    {
        IsLevitating = false;
        
        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();

        if (levitateable != null)
        {
            levitateable.State = LevitationState.NotLevitating;
        }

        RemoveSelectedRigidbody();
    }

    private void RemoveSelectedRigidbody()
    {
        IsLevitating = false;
        _selectedRigidbody = null;
    }
}
