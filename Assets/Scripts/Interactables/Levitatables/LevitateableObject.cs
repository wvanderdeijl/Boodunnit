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
    
    //TODO duplicate code
    private void FreezeObject()
    {
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
        CanBeLevitated = false;
        State = LevitationState.Frozen;
    }

    private void ReleaseObject()
    {
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        CanBeLevitated = true;
        State = LevitationState.NotLevitating;
    }
    
    public IEnumerator LevitateForSeconds(float seconds)
    {
        FreezeObject();
        yield return new WaitForSeconds(seconds);
        ReleaseObject();
    }
}
