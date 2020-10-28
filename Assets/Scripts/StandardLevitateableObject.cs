using System.Collections;
using UnityEngine;

public enum LevitationState
{
 Levitating,
 NotLevitating
}

[RequireComponent(typeof(Rigidbody))]
public class StandardLevitateableObject : MonoBehaviour, ILevitateable
{
    private float _secondsToLevitate = 5f;
    private Rigidbody _rigidbodyObject;

    private LevitationState _currentLevitationState = LevitationState.NotLevitating;
    
    
    private void Awake()
    {
        _rigidbodyObject = GetComponent<Rigidbody>();
        CanBeLevitated = true;
    }

    private void FreezeObject()
    {
        _rigidbodyObject.useGravity = false;
        _rigidbodyObject.isKinematic = true;
        CanBeLevitated = false;
        _currentLevitationState = LevitationState.Levitating;
    }

    private void ReleaseObject()
    {
        _rigidbodyObject.useGravity = true;
        _rigidbodyObject.isKinematic = false;
        CanBeLevitated = true;
        _currentLevitationState = LevitationState.NotLevitating;
    }

    public void StartLevitation()
    {
        if (_currentLevitationState == LevitationState.NotLevitating)
        {
            StartCoroutine(LevitateForSeconds(_secondsToLevitate));
        }
    }

    public IEnumerator LevitateForSeconds(float seconds)
    {
        FreezeObject();
        yield return new WaitForSeconds(seconds);
        ReleaseObject();
    }

    public void ToggleCanBeLevitated()
    {
        CanBeLevitated = !CanBeLevitated;
    }

    public bool CanBeLevitated { get; set; }
    
    public LevitationState State { get; set; }
}
