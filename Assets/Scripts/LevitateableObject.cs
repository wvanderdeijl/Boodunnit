using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LevitateableObject : MonoBehaviour, ILevitateable
{
    private Rigidbody _rigidbody;
    
    private void Awake()
    {
        CanBeLevitated = true;
        IsInsideSphere = false;
        State = LevitationState.NotLevitating;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public bool CanBeLevitated { get; set; }
    
    public bool IsInsideSphere { get; set; }

    public LevitationState State { get; set; }

    private void FreezeOrReleaseLevitateableObject(LevitationState levitationState)
    {
        //TODO: fix duplicate code
        
        switch (levitationState)
        {
            case LevitationState.NotLevitating:
                _rigidbody.useGravity = false;
                _rigidbody.isKinematic = true;
                CanBeLevitated = false;
                break;
            
            case LevitationState.Frozen:
                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;
                CanBeLevitated = true;
                break;
        }
        
        State = levitationState;
    }

    public IEnumerator LevitateForSeconds(float seconds)
    {
        FreezeOrReleaseLevitateableObject(LevitationState.NotLevitating);
        yield return new WaitForSeconds(seconds);
        FreezeOrReleaseLevitateableObject(LevitationState.Frozen);
    }
}
