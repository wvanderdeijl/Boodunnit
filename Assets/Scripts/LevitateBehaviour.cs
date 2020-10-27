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

    public void ReceiveObjectRigidbodyWithMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _selectedRigidbody = GetRigidbodyFromMouseClick();
        }
        
        if (Input.GetMouseButtonUp(0) && _selectedRigidbody)
        {
            _selectedRigidbody = null;
        }
    }

    public void MoveLevitateableObject()
    {
        if (_selectedRigidbody)
        {
            if (!LevitateableObjectIsInRange())
            {
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
    
    private Rigidbody GetRigidbodyFromMouseClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        bool hit = Physics.Raycast(ray, out hitInfo);
        
        if (hit)
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
}
