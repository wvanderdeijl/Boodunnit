using DefaultNamespace.Enums;
using UnityEngine;
using UnityEngine.Audio;

public class LevitateBehaviour : MonoBehaviour
{
    [SerializeField] private float _levitateRange = 10f;
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;
    
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
        
        if (!_selectedRigidbody.gameObject.GetComponent<ILevitateable>().IsInsideSphere)
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
            //todo: Mathf.Clamp
                
            _selectionDistance = 2.1f;
            return;
        }
            
        _selectionDistance += (Input.GetAxis("Mouse ScrollWheel") * 300f * Time.deltaTime);
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
        ActivateLevitateCoRoutine();

        _selectedRigidbody.gameObject.GetComponent<ILevitateable>().State = LevitationState.NotLevitating;
        
        _selectedRigidbody = null;
    }

    private Rigidbody GetRigidbodyFromMouseClick()
    {
        if (_selectedRigidbody)
        {
            if (_selectedRigidbody.gameObject.GetComponent<ILevitateable>().State == LevitationState.Levitating)
            {
                return null;
            }

            if (!_selectedRigidbody.gameObject.GetComponent<ILevitateable>().IsInsideSphere)
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
        _hitColliders = Physics.OverlapSphere(_player.transform.position, 5f);

        if (_colliderCount > 0)
        {
            foreach (var hitCollider in _cachedHitColliders)
            {
                if (hitCollider.gameObject.GetComponent(typeof(ILevitateable)))
                {
                    Vector3 targetDirection = hitCollider.transform.position - transform.position;
                    float angle = Vector3.Angle(targetDirection, transform.forward);

                    if (angle > -30f || angle < 30f) //todo: wat de knekker moet ik voor waarden in vullen.
                    {
                        Debug.Log(hitCollider.gameObject.name);
                        hitCollider.gameObject.GetComponent<ILevitateable>().IsInsideSphere = false;
                    }
                }
            }
        }

        foreach (var hitCollider in _hitColliders)
        {
            if (hitCollider.gameObject.GetComponent(typeof(ILevitateable)))
            {
                hitCollider.gameObject.GetComponent<ILevitateable>().IsInsideSphere = true;
            }
        }

        _cachedHitColliders = _hitColliders;
        _colliderCount++;
    }

    private void ActivateLevitateCoRoutine()
    {
        if (_selectedRigidbody && _selectedRigidbody.transform.gameObject.GetComponent(typeof(ILevitateable)))
        {
            StartCoroutine(_selectedRigidbody.transform.gameObject.GetComponent<ILevitateable>().LevitateForSeconds(5f));
        }
    }
}
