using DefaultNamespace.Enums;
using UnityEngine;
using System.Linq;

public class LevitateBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;

    [Header("OverlapSphere")]
    [SerializeField] private float _overlapSphereRadiusInUnits = 12f;
    [SerializeField][Range(0, 360)] private float _overlapSphereAngleInDegrees = 360f;
    
    [Header("Speeds")]
    [SerializeField]private float _velocitySpeedPercentage = 25f;
    [SerializeField] private float _pushPullSpeed = 300f;
    [SerializeField] private float _rotationSpeed = 5f;
    
    [Header("Durations")]
    [SerializeField] private float _frozenDurationInSeconds = 5f;

    [SerializeField] private LayerMask _layerMask;

    public static bool IsRotating { get; set; }
    public bool IsLevitating { get; set; }
    public Collider[] CurrentLevitateableObjects { get; set; }
    public bool IsPushing { get; set; }

    private Rigidbody _selectedRigidbody;
    
    private float _heightOfLevitateableObject;
    private float _distanceOfLevitateableObject = 10f;

    private Vector3 _originalScreenTargetPosition;
    private Vector3 _originalRigidbodyPosition;

    public void LevitationStateHandler()
    {
        if (!_selectedRigidbody)
        {
            FindObjectOfType<CameraController>().CanScrollZoom = false;
            FindObjectOfType<CameraController>().CanAutoZoom = false;
            GetRigidbodyAndStartLevitation();
            DisableRotation(true);
        }

        else
        {
            FindObjectOfType<CameraController>().CanScrollZoom = true;
            FindObjectOfType<CameraController>().CanAutoZoom = true;
            ToggleGravity(true);
            IsPushing = false;
            DisableRotation(false);
            _heightOfLevitateableObject = 0f;
            _distanceOfLevitateableObject = 10f;
            RemoveRigidbodyAndStartFreeze();
        }
    }

    public void ToggleMiddleMouseButton()
    {
        IsPushing = !IsPushing;
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
        
        ToggleGravity(false);

        Transform cameraTransform = Camera.main.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        Vector3 endOfRayCast = ray.GetPoint(_distanceOfLevitateableObject);
        Vector3 heightOffset = new Vector3(0, _heightOfLevitateableObject * Time.deltaTime * 10f, 0);
        Vector3 targetPosition = endOfRayCast + heightOffset;

        // _selectedRigidbody.transform.forward = ray.direction;

        _selectedRigidbody.MovePosition(targetPosition);
    }

    private void ToggleGravity(bool useGravity)
    {
        if (!_selectedRigidbody) return;

        _selectedRigidbody.useGravity = useGravity;
    }

    public void ChangeHeightOfLevitateableObject()
    {
        if (!_selectedRigidbody) return;

        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        
        if (scrollWheelInput > 0 || scrollWheelInput < 0)
        {
            _heightOfLevitateableObject += (scrollWheelInput * _pushPullSpeed); 
        }
    }
    
    public void PushOrPullLevitateableObject()
    {
        if (!_selectedRigidbody) return;
        
        if (_distanceOfLevitateableObject < 10f)
        {
            _distanceOfLevitateableObject = 10.1f;
            return;
        }

        _distanceOfLevitateableObject += (Input.GetAxis("Mouse ScrollWheel") * _pushPullSpeed * Time.deltaTime);
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

    public void DisableRotation(bool disable)
    {
        if (!_selectedRigidbody) return;

        switch (disable)
        {
            case true:
                _selectedRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                break;
            case false:
                _selectedRigidbody.constraints = RigidbodyConstraints.None;
                break;
        }
    }

    private void GetRigidbodyAndStartLevitation()
    {
        _selectedRigidbody = GetRigidbodyFromMouseClick();
        if (!_selectedRigidbody) return;
        IsLevitating = true;
        ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();
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

        if (Physics.Raycast(_mainCamera.transform.position, _mainCamera.transform.forward, out RaycastHit hitInfo, 100f, ~_layerMask))
        {
            Rigidbody rigidbody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();

            if (!rigidbody) return null;
            if (!hitInfo.collider.gameObject.GetComponent(typeof(ILevitateable))) return null;

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
        _heightOfLevitateableObject = 0;
    }
}
