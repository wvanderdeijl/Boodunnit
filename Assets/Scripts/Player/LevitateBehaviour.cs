using DefaultNamespace.Enums;
using UnityEngine;
using System.Linq;

public class LevitateBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;

    [Header("OverlapSphere")]
    [SerializeField][Range(0, 30)] private float _overlapSphereRadiusInUnits = 5f;
    [SerializeField][Range(0, 360)] private float _overlapSphereAngleInDegrees = 360f;
    
    [Header("Speeds")]
    [SerializeField][Range(0, 100)] private float _velocitySpeedPercentage = 25f;
    [SerializeField][Range(0, 500)] private float _pushPullSpeed = 300f;
    [SerializeField][Range(0, 50)] private float _rotationSpeed = 5f;
    
    [Header("Durations")]
    [SerializeField] private float _frozenDurationInSeconds = 5f;
    
    [Header("Distances")]
    [SerializeField] private float _minimumSelectionDistanceInUnits = 2f;
    
    public static bool IsRotating { get; set; }

    private Rigidbody _selectedRigidbody;
    private float _selectionDistance;
    
    private Vector3 _originalScreenTargetPosition;
    private Vector3 _originalRigidbodyPosition;

    private Collider[] _hitColliders;
    private Collider[] _cachedHitColliders;
    private int _colliderCount;
    
    public void LevitationStateHandler()
    {
        if (!_selectedRigidbody)
        {
            GetRigidbodyAndChangeState();
        }

        else
        {
            RemoveRigidbodyAndChangeState();
        }
    }
    
    public void MoveLevitateableObject()
    {
        if (!_selectedRigidbody) return;

        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();

        if (!levitateable.IsInsideSphere ||
            !levitateable.CanBeLevitated)
        {
            RemoveGameObjectFromCursor();
            return;
        }

        Vector3 mousePosition = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            _selectionDistance
        );
            
        Vector3 mousePositionOffset = 
            _mainCamera.ScreenToWorldPoint(mousePosition) - _originalScreenTargetPosition;
            
        _selectedRigidbody.velocity = 
            (_originalRigidbodyPosition + mousePositionOffset - _selectedRigidbody.transform.position) 
            * (500 * Time.deltaTime * (_velocitySpeedPercentage / 100));
    }

    private void RemoveGameObjectFromCursor()
    {
        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();

        if (levitateable != null)
        {
            levitateable.State = LevitationState.NotLevitating;
        }

        _selectedRigidbody = null;
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

    public void RotateLevitateableObject()
    {
        if (!_selectedRigidbody) return;

        if (IsRotating)
        {
            _selectedRigidbody.useGravity = false;
            _selectedRigidbody.isKinematic = true;
        }
        else
        {
            _selectedRigidbody.useGravity = true;
            _selectedRigidbody.isKinematic = false;
        }

        float xaxisRotation = Input.GetAxis("Mouse X")* _rotationSpeed * Time.deltaTime;
        float yaxisRotation = Input.GetAxis("Mouse Y")* _rotationSpeed * Time.deltaTime;
            
        _selectedRigidbody.transform.RotateAround (Vector3.down, xaxisRotation);
        _selectedRigidbody.transform.RotateAround (_mainCamera.transform.rotation * Vector3.right, yaxisRotation);
    }

    private void GetRigidbodyAndChangeState()
    {
        _selectedRigidbody = GetRigidbodyFromMouseClick();

        if (!_selectedRigidbody) return;
        
        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();

        if (levitateable != null)
        {
            levitateable.State = LevitationState.Levitating;
        }
    }

    public void RemoveRigidbodyAndChangeState()
    {
        ILevitateable levitateable =
            _selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<ILevitateable>() : null;
        
        if (levitateable != null)
        {
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
            
            levitateable.State = LevitationState.Frozen;
        }

        ActivateLevitateCoRoutine();
        
        _selectedRigidbody = null;
    }

    private Rigidbody GetRigidbodyFromMouseClick()
    {
        if (_selectedRigidbody)
        {
            ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();
            
            if (levitateable != null &&
                (!levitateable.IsInsideSphere ||
                !levitateable.CanBeLevitated ||
                levitateable.State == LevitationState.Frozen))
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

    public void FindObjectInFrontOfPLayer()
    {
        _hitColliders = Physics.OverlapSphere(_player.transform.position, _overlapSphereRadiusInUnits);
        
        if (_colliderCount > 0)
        {
            foreach (var hitCollider in _cachedHitColliders)
            {
                ILevitateable levitateable = hitCollider.gameObject.GetComponent<ILevitateable>();        
                if (levitateable != null)
                {
                    ToggleIsInsideSphereBool(hitCollider, false);
                }
            }
        }
    
        foreach (var hitCollider in _hitColliders)
        {
            ILevitateable levitateable = hitCollider.gameObject.GetComponent<ILevitateable>();        
            if (levitateable != null)
            {
                ToggleIsInsideSphereBool(hitCollider, true);
            }
        }
    
        _cachedHitColliders = _hitColliders;
        _colliderCount++;
    }
    
    private void ToggleIsInsideSphereBool(Collider hitCollider, bool isInsideSphere)
    {
        Vector3 targetDirection = hitCollider.transform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, _player.transform.forward);
                
        if (angle > -(_overlapSphereAngleInDegrees / 2) && angle < _overlapSphereAngleInDegrees / 2)
        {
            ILevitateable levitateable = hitCollider.gameObject.GetComponent<ILevitateable>();
            if (levitateable != null)
            {
                levitateable.IsInsideSphere = isInsideSphere;
            }
        }
    }

    private void ActivateLevitateCoRoutine()
    {
        ILevitateable levitateable =_selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<ILevitateable>() : null;     
        if (_selectedRigidbody && levitateable != null)
        {
            StartCoroutine(levitateable.LevitateForSeconds(_frozenDurationInSeconds));
        }
    }
    
    
    
    
    
    // // GETTING ALL LEVITATEABLE OBJECTS IN ARRAY
    // public void FindLevitateableObjectsInFrontOfPlayer()
    // {
    //     Collider[] colliders = Physics
    //         .OverlapSphere(transform.position, _overlapSphereRadiusInUnits)
    //         .Where(c => { return IsObjectInRange(c) && IsObjectLevitateble(c) && IsObjectInAngle(c); })
    //         .ToArray();
    //
    //     foreach (Collider collider in colliders)
    //     {
    //         ILevitateable levitateable = collider.gameObject.GetComponent<ILevitateable>();
    //         levitateable
    //     }
    // }
    //
    // private bool IsObjectInRange(Collider collider)
    // {
    //     return Vector3.Distance(transform.position, collider.transform.position) <= _overlapSphereRadiusInUnits;
    // }
    //
    // private bool IsObjectLevitateble(Collider collider)
    // {
    //     ILevitateable levitateableObject = collider.GetComponent<ILevitateable>();
    //     return levitateableObject != null;
    // }
    //
    // private bool IsObjectInAngle(Collider collider)
    // {
    //     Vector3 colliderDirection = collider.transform.position - transform.position;
    //     float angle = Vector3.Angle(colliderDirection, _player.transform.forward);
    //     return angle > -(_overlapSphereAngleInDegrees / 2) && angle < _overlapSphereAngleInDegrees / 2;
    // }
}
