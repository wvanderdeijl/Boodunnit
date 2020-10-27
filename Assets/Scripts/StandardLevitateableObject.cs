using System.Collections;
using UnityEngine;

public class StandardLevitateableObject : MonoBehaviour
{
    private int _secondsToLevitate = 5;
    private Rigidbody _rigidbodyObject;
    
    private enum LevitationState { IsLevitated, IsNotLevitated }
    private LevitationState _currentLevitationState = LevitationState.IsNotLevitated;
    
    private void Awake()
    {
        _rigidbodyObject = GetComponent<Rigidbody>();
    }

    private void FreezeObject()
    {
        _rigidbodyObject.useGravity = false;
        _rigidbodyObject.isKinematic = true;
        _currentLevitationState = LevitationState.IsLevitated;
    }

    private void ReleaseObject()
    {
        _rigidbodyObject.useGravity = false;
        _rigidbodyObject.isKinematic = true;
        _currentLevitationState = LevitationState.IsNotLevitated;
    }
    
    public IEnumerator LevitateForSeconds(int seconds)
    {
        FreezeObject();
        yield return new WaitForSeconds(seconds);
        ReleaseObject();
    }

    public void StartLevitation()
    {
        if (_currentLevitationState == LevitationState.IsNotLevitated)
        {
            StartCoroutine(LevitateForSeconds(_secondsToLevitate));
        }
    }
}
