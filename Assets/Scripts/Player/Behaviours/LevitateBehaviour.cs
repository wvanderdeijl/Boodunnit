using DefaultNamespace.Enums;
using UnityEngine;
using UnityEngine.Audio;

public class LevitateBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _mouseWheelSpeed = 300f;
    [SerializeField] private float _overlapSphereRadius = 5f;
    [SerializeField] private float _minimumSelectionDistance = 2f;
    [SerializeField] private float _rotationSpeed = 30f;
    [SerializeField] private float _frozenDuration = 5f;
    [SerializeField] private float _overlapSphereAngle = 45f;
    
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
            * (500 * Time.deltaTime);
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
        
        if (_selectionDistance < _minimumSelectionDistance)
        {
            _selectionDistance = _minimumSelectionDistance + 0.1f;
            return;
        }
            
        _selectionDistance += (Input.GetAxis("Mouse ScrollWheel") * _mouseWheelSpeed * Time.deltaTime);
    }

    public void RotateLevitateableObject()
    {
        if (!_selectedRigidbody) return;

        float xaxisRotation = Input.GetAxis("Mouse X")* _rotationSpeed * Time.deltaTime;
        float yaxisRotation = Input.GetAxis("Mouse Y")* _rotationSpeed * Time.deltaTime;
            
        _selectedRigidbody.transform.RotateAround (Vector3.down, xaxisRotation);
        _selectedRigidbody.transform.RotateAround (Vector3.right, yaxisRotation);
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
        
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out hitInfo);

        if (!hitInfo.collider) return null;
            
        Rigidbody rigidbody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();

        if (!hit || !hitInfo.collider.gameObject.GetComponent(typeof(ILevitateable))) return null;

        if (!rigidbody) return null;
        
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


    public void FindObjectInFrontOfPLayer()
    {
        _hitColliders = Physics.OverlapSphere(_player.transform.position, _overlapSphereRadius);
        
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
                
        if (angle > -_overlapSphereAngle && angle < _overlapSphereAngle)
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
        ILevitateable levitateable =
            _selectedRigidbody ? _selectedRigidbody.gameObject.GetComponent<ILevitateable>() : null;
        
        if (_selectedRigidbody && levitateable != null)
        {
            StartCoroutine(levitateable.LevitateForSeconds(_frozenDuration));
        }
    }
}