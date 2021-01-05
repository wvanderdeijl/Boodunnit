using System;
using DefaultNamespace.Enums;
using UnityEngine;
using System.Linq;

public class LevitateBehaviour : MonoBehaviour
{
    [Header("OverlapSphere")]
    [SerializeField] private float _overlapSphereRadiusInUnits = 15f;
    [SerializeField][Range(0, 360)] private float _overlapSphereAngleInDegrees = 360f;

    [Header("Durations")]
    [SerializeField] private float _frozenDurationInSeconds = 5f;

    [Header("Layermasks")]
    [SerializeField] private LayerMask _layerMask;

    private GameObject _player;
    private Camera _mainCamera; 
    private Rigidbody _selectedRigidbody;
    private float _heightOfLevitateableObject = 4f;
    private float _distanceOfLevitateableObject = 10f;
    private Vector3 _originalScreenTargetPosition;
    
    public bool IsLevitating { get; set; }
    public Collider[] CurrentLevitateableObjects { get; set; }
    public bool PushingObjectIsToggled { get; set; }

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _mainCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<Camera>();
    }

    #region Levitati on handler
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
        // FindObjectOfType<CameraController>().CanScrollZoom = false;
        // FindObjectOfType<CameraController>().CanAutoZoom = false;
        GetRigidbodyAndStartLevitation();
    }

    private void StopLevitation()
    {
        // FindObjectOfType<CameraController>().CanScrollZoom = true;
        // FindObjectOfType<CameraController>().CanAutoZoom = true;
        ToggleGravity(true);
        PushingObjectIsToggled = false;
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

    
    
    
    #region Finding Levitateable Objects In Front Of Player.
    public void FindLevitateableObjectsInFrontOfPlayer()
    {
        CurrentLevitateableObjects = Physics
            .OverlapSphere(transform.position, _overlapSphereRadiusInUnits)
            .Where(c => { return IsObjectInRange(c) && IsObjectLevitateble(c) && IsObjectInAngle(c); })
            .ToArray();
    }
    
    private bool IsObjectInRange(Collider colliderParam)
    {
        return Vector3.Distance(transform.position, colliderParam.transform.position) <= _overlapSphereRadiusInUnits;
    }
    
    private bool IsObjectLevitateble(Collider colliderParam)
    {
        ILevitateable levitateableObject = colliderParam.GetComponent<ILevitateable>();
        return levitateableObject != null;
    }
    
    private bool IsObjectInAngle(Collider colliderParam)
    {
        Vector3 colliderDirection = colliderParam.transform.position - transform.position;
        float angle = Vector3.Angle(colliderDirection, _player.transform.forward);
        return angle > -(_overlapSphereAngleInDegrees / 2) && angle < _overlapSphereAngleInDegrees / 2;
    }

    private bool IsObjectInLevitateablesArray(Collider colliderParam)
    {
        return CurrentLevitateableObjects.Length > 0 && CurrentLevitateableObjects.Contains(colliderParam);
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

    private void ToggleGravity(bool useGravity)
    {
        if (!_selectedRigidbody) return;

        _selectedRigidbody.useGravity = useGravity;
    }
    #endregion
}