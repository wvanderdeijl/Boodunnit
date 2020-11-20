using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LevitateableObject : MonoBehaviour, ILevitateable
{
    [SerializeField] private bool _canRespawnWhenOutOfRange;
    public float DespawnDistance = 10f;
    private Vector3 _spawnLocation;
    private Quaternion _spawnRotation;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        CanBeLevitated = true;
        IsInsideSphere = false;
        State = LevitationState.NotLevitating;
        _rigidbody = GetComponent<Rigidbody>();
        
        _spawnLocation = transform.position;
        _spawnRotation = transform.rotation;
        StartCoroutine(CheckForDistance());
    }

    public bool CanBeLevitated { get; set; }

    public bool CanRespawnWhenOutOfRange
    {
        get
        {
            return _canRespawnWhenOutOfRange;
        }
        set
        {
            _canRespawnWhenOutOfRange = value;
        }
    }

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

    private IEnumerator CheckForDistance()
    {
        yield return new WaitForSeconds(3f);
        if (CanRespawnWhenOutOfRange && Mathf.Abs(Vector3.Distance(transform.position, _spawnLocation)) >= DespawnDistance && Mathf.Abs(Vector3.Distance(transform.position, CameraController.RotationTarget.position)) >= DespawnDistance && Mathf.Abs(Vector3.Distance(_spawnLocation, CameraController.RotationTarget.position)) >= DespawnDistance)
        {
            Despawn();
        }

        StartCoroutine(CheckForDistance());
    }
    
    public void Despawn()
    {
        print("yeet");
        transform.position = _spawnLocation;
        transform.rotation = _spawnRotation;
        _rigidbody.velocity = Vector3.zero;
    }
}
