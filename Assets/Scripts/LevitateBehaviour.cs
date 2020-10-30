using DefaultNamespace.Enums;
using UnityEngine;
using UnityEngine.Audio;

public class LevitateBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _mouseWheelSpeed = 300f;
    [SerializeField] private float _overlapSphereRadius = 5f;
    
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
        _selectedRigidbody.gameObject.GetComponent<ILevitateable>().State = LevitationState.NotLevitating;
        _selectedRigidbody = null;
    }

    public void PushOrPullLevitateableObject()
    {
        if (!_selectedRigidbody) return;
        
        if (_selectionDistance < 2f)
        {
            _selectionDistance = 2.1f;
            return;
        }
            
        _selectionDistance += (Input.GetAxis("Mouse ScrollWheel") * _mouseWheelSpeed * Time.deltaTime);
    }

    public void RotateLevitateableObject()
    {
        if (!_selectedRigidbody) return;

        float xaxisRotation = Input.GetAxis("Mouse X")* 30f * Time.deltaTime;
        float yaxisRotation = Input.GetAxis("Mouse Y")* 30f * Time.deltaTime;
            
        _selectedRigidbody.transform.RotateAround (Vector3.down, xaxisRotation);
        _selectedRigidbody.transform.RotateAround (Vector3.right, yaxisRotation);
    }

    private void GetRigidbodyAndChangeState()
    {
        _selectedRigidbody = GetRigidbodyFromMouseClick();

        if (!_selectedRigidbody) return;
                
        _selectedRigidbody.gameObject.GetComponent<ILevitateable>().State = LevitationState.Levitating;
    }

    private void RemoveRigidbodyAndChangeState()
    {
        _selectedRigidbody.gameObject.GetComponent<ILevitateable>().State = LevitationState.Frozen;

        ActivateLevitateCoRoutine();
        
        _selectedRigidbody = null;
    }

    private Rigidbody GetRigidbodyFromMouseClick()
    {
        if (_selectedRigidbody)
        {

            ILevitateable levitateable = _selectedRigidbody.gameObject.GetComponent<ILevitateable>();
            
            if (levitateable.State == LevitationState.Frozen)
            {
                return null;
            }

            if (!levitateable.IsInsideSphere ||
                !levitateable.CanBeLevitated)
            {
                return null;
            }
        }
        
        RaycastHit hitInfo = new RaycastHit();

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        bool hit = Physics.Raycast(ray, out hitInfo);

        if (!hit || !hitInfo.collider.gameObject.GetComponent(typeof(ILevitateable))) return null;
        if (!hitInfo.collider.gameObject.GetComponent<Rigidbody>()) return null;

        _selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);

        _originalScreenTargetPosition = _mainCamera.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                _selectionDistance
            )
        );

        _originalRigidbodyPosition = hitInfo.collider.transform.position;

        return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
    }


    public void FindObjectInFrontOfPLayer()
    {
        _hitColliders = Physics.OverlapSphere(_player.transform.position, _overlapSphereRadius);
        
        if (_colliderCount > 0)
        {
            foreach (var hitCollider in _cachedHitColliders)
            {
                if (hitCollider.gameObject.GetComponent(typeof(ILevitateable)))
                {
                    ToggleIsInsideSphereBool(hitCollider, false);
                }
            }
        }

        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider.gameObject.GetComponent(typeof(ILevitateable)))
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
                
        if (angle > -45f && angle < 45f)
        {
            hitCollider.gameObject.GetComponent<ILevitateable>().IsInsideSphere = isInsideSphere;
        }
    }

    private void ActivateLevitateCoRoutine()
    {
        if (_selectedRigidbody && _selectedRigidbody.transform.gameObject.GetComponent(typeof(ILevitateable)))
        {
            StartCoroutine(_selectedRigidbody.transform.gameObject.GetComponent<ILevitateable>().LevitateForSeconds(5f));
            StartCoroutine(_selectedRigidbody.transform.gameObject.GetComponent<ILevitateable>().LevitateForSeconds(5f));
        }
    }
}
