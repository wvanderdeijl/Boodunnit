using DefaultNamespace.Enums;
using UnityEngine;
using System.Linq;

public class LevitateBehaviourStaticLevitation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;

    [Header("OverlapSphere")]
    [SerializeField] private float _overlapSphereRadiusInUnits = 20f;
    [SerializeField][Range(0, 360)] private float _overlapSphereAngleInDegrees = 360f;
    
    [Header("Speeds")]
    [SerializeField] private float _pushPullSpeed = 300f;
    [SerializeField] private float _rotationSpeed = 5f;
    
    [Header("Durations")]
    [SerializeField] private float _frozenDurationInSeconds = 5f;

    [Header("Layermasks")]
    [SerializeField] private LayerMask _layerMask;

    public static bool IsRotating { get; set; }
    public static bool IsLevitating { get; set; }
    public Collider[] CurrentLevitateableObjects { get; set; }
    public bool PushingObjectIsToggled { get; set; }

    private Rigidbody _selectedRigidbody;
    private float _heightOfLevitateableObject = 4f;
    private float _distanceOfLevitateableObject = 10f;
    private Vector3 _originalScreenTargetPosition;
    
    #region Levitation handler
    public void LevitationStateHandler()
    {
        if (!_selectedRigidbody)
        {
            Levitate();
        }

        else
        {
            StopLevitation();
        }
    }

    private void Levitate()
    {
        FindObjectOfType<CameraController>().CanScrollZoom = false;
        FindObjectOfType<CameraController>().CanAutoZoom = false;
        GetRigidbodyAndStartLevitation();
        DisableRotation(true);
    }

    private void StopLevitation()
    {
        FindObjectOfType<CameraController>().CanScrollZoom = true;
        FindObjectOfType<CameraController>().CanAutoZoom = true;
        ToggleGravity(true);
        PushingObjectIsToggled = false;
        DisableRotation(false);
        _heightOfLevitateableObject = 4f;
        _distanceOfLevitateableObject = 10f;
        RemoveRigidbodyAndStartFreeze();
    }
    #endregion
    
    #region Handle Movement
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
        _selectedRigidbody.MovePosition(targetPosition);
    }
    #endregion

    #region Handle push/pull and height
    public void ChangeHeightOfLevitateableObject()
    {
        if (!_selectedRigidbody) return;

        float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        
        if (scrollWheelInput > 0 || scrollWheelInput < 0)
        {
            _heightOfLevitateableObject += (scrollWheelInput * _pushPullSpeed);
            _heightOfLevitateableObject = Mathf.Clamp(_heightOfLevitateableObject, 4f, _overlapSphereRadiusInUnits);
        }
    }
    
    public void ChangeDistanceOfLevitateableObject()
    {
        if (!_selectedRigidbody) return;
        _distanceOfLevitateableObject += (Input.GetAxis("Mouse ScrollWheel") * _pushPullSpeed * Time.deltaTime);
        _distanceOfLevitateableObject = Mathf.Clamp(_distanceOfLevitateableObject, 10, _overlapSphereRadiusInUnits);
    }
    #endregion
    
    #region Handle rotation
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
    #endregion

    #region Handle rigidbody
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
            
            // changing layers (default layer changes back within the levitateable object script.
            int levitatingObjectLayerMask = LayerMask.NameToLayer("LevitatingObject");
            foreach (Transform transform in rigidbody.GetComponentsInChildren<Transform>())
            {
                transform.gameObject.layer = levitatingObjectLayerMask;
            }     
            
            return rigidbody;
        }

        return null;
    }
    #endregion

    #region Handle object freeze
    private void ActivateLevitateCoRoutine()
    {
        ILevitateable levitateable =
            _selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<ILevitateable>() : null;   
        
        if (_selectedRigidbody && levitateable != null)
        {
            StartCoroutine(levitateable.LevitateForSeconds(_frozenDurationInSeconds));
        }
    }
    #endregion

    #region Handle levitateable objectes seeker
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
    #endregion

    #region Misc
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
        _heightOfLevitateableObject = 4f;
    }
    
    public void ToggleMiddleMouseButton()
    {
        PushingObjectIsToggled = !PushingObjectIsToggled;
    }
    
    private void ToggleGravity(bool useGravity)
    {
        if (!_selectedRigidbody) return;

        _selectedRigidbody.useGravity = useGravity;
    }
    #endregion
}