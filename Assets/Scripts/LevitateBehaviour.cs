using System.Collections;
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
    
    private enum LevitationState { IsLevitating, IsNotLevitating }
    private LevitationState _currentLevitationState = LevitationState.IsNotLevitating;
    
    public void LevitationStateHandler()
    {
        switch (_currentLevitationState)
        {
            case LevitationState.IsNotLevitating:
                GetRigidbodyAndChangeState();
                break;
            case LevitationState.IsLevitating:
                StartCoroutine(RemoveRigidbodyAndChangeState());
                break;
        }
    }
    
    public void MoveLevitateableObject()
    {
        if (_selectedRigidbody)
        {
            if (!LevitateableObjectIsInRange())
            {
                _selectedRigidbody = null;
                _currentLevitationState = LevitationState.IsNotLevitating;
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
            //todo: push or pull with scroll bar 
        }
    }

    public void RotateLevitateableObject()
    {
        if (_selectedRigidbody)
        {
            //todo: rotate object by dragging mouse
        }
    }

    private void GetRigidbodyAndChangeState()
    {
        _selectedRigidbody = GetRigidbodyFromMouseClick();

        if (!_selectedRigidbody) return;
                
        _currentLevitationState = LevitationState.IsLevitating;
    }

    private IEnumerator RemoveRigidbodyAndChangeState()
    {
        ActivateLevitateCoRoutine();
        
        yield return new WaitForSeconds(5f);
        
        _currentLevitationState = LevitationState.IsNotLevitating;
        
        _selectedRigidbody = null;
    }
    
    private Rigidbody GetRigidbodyFromMouseClick()
    {
        if (_currentLevitationState == LevitationState.IsLevitating) return null;

        RaycastHit hitInfo = new RaycastHit();
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        
        bool hit = Physics.Raycast(ray, out hitInfo);
        
        //todo: also check if the gameobject has an Ilevatateable Interface
        
        if (hit && hitInfo.collider.gameObject.GetComponent<StandardLevitateableObject>())
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

    private bool IsLevitateableObjectInFrontOfPlayer()
    {
        //todo: check if objects are in front of the player
        return false;
    }
    
    private void ActivateLevitateCoRoutine()
    {
        if (_selectedRigidbody)
        {
            _selectedRigidbody.transform.gameObject.GetComponent<StandardLevitateableObject>().StartLevitation();
            
            //todo: Start the levitate CoRoutine if the game object has an ILevitateable interface
        }
    }
}
