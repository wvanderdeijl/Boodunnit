using DefaultNamespace.Enums;
using UnityEngine;

public class LevitateBehaviour : MonoBehaviour
{
    [SerializeField] private float _levitateRange = 10f;
    [SerializeField] private GameObject _player;
    [SerializeField] private Camera _mainCamera;
    
    private Rigidbody _selectedRigidbody;
    private float _selectionDistance;
    
    private Vector3 _originalScreenTargetPosition;
    private Vector3 _originalRigidbodyPosition;

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
        if (_selectedRigidbody)
        {
            if (!LevitateableObjectIsInRange())
            {
                _selectedRigidbody.gameObject.GetComponent<ILevitateable>().State = LevitationState.NotLevitating;
                _selectedRigidbody = null;
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
    }

    public void PushOrPullLevitateableObject()
    {
        if (_selectedRigidbody)
        {
            if (_selectionDistance < 2f)
            {
                //todo: Mathf.Clamp
                
                _selectionDistance = 2.1f;
                return;
            }
            
            _selectionDistance += (Input.GetAxis("Mouse ScrollWheel") * 300f * Time.deltaTime);
        }
    }

    public void RotateLevitateableObject()
    {
        if (_selectedRigidbody)
        {
            float XaxisRotation = Input.GetAxis("Mouse X")* 30f * Time.deltaTime;
            float YaxisRotation = Input.GetAxis("Mouse Y")* 30f * Time.deltaTime;
            _selectedRigidbody.transform.RotateAround (Vector3.down, XaxisRotation);
            _selectedRigidbody.transform.RotateAround (Vector3.right, YaxisRotation);
        }
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
        if(_selectedRigidbody)
        {
            if (_selectedRigidbody.gameObject.GetComponent<ILevitateable>().State == LevitationState.Levitating)
            {
                return null;
            }

            if (!_selectedRigidbody.gameObject.GetComponent<ILevitateable>().CanBeLevitated)
            {
                return null;
            }
        } 
            

        RaycastHit hitInfo = new RaycastHit();
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        bool hit = Physics.Raycast(ray, out hitInfo);

        if (hit && hitInfo.collider.gameObject.GetComponent(typeof(ILevitateable)) )
        {
            if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
            {
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
        }

        return null;
    }

    private bool LevitateableObjectIsInRange()
    {
        float distance = Vector3.Distance(_player.transform.position, _selectedRigidbody.transform.position);
        return distance < _levitateRange;
    }

    private void ActivateLevitateCoRoutine()
    {
        if (_selectedRigidbody && _selectedRigidbody.transform.gameObject.GetComponent(typeof(ILevitateable)))
        {
            StartCoroutine(_selectedRigidbody.transform.gameObject.GetComponent<ILevitateable>().LevitateForSeconds(5f));
        }
    }
}
