using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

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
        State = LevitationState.NotLevitating;
        _rigidbody = GetComponent<Rigidbody>();
        
        _spawnLocation = transform.position;
        _spawnRotation = transform.rotation;
        StartCoroutine(CheckForDistance());

        Outline outline = gameObject.AddComponent<Outline>();
        if (outline)
        {
            Color purple;
            ColorUtility.TryParseHtmlString("#d2b8db", out purple);

            outline.OutlineColor = purple;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 5.0f;
            outline.enabled = false;
        }
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

    public LevitationState State { get; set; }

    private void FreezeOrReleaseLevitateableObject(LevitationState levitationState)
    {
        switch (levitationState)
        {
            case LevitationState.NotLevitating:
                SetRigidbodyAndLevitationBooleans(false, true, false);
                break;
            
            case LevitationState.Frozen:
                SetRigidbodyAndLevitationBooleans(true, false, true);
                break;
        }
        
        State = levitationState;
    }

    private void SetRigidbodyAndLevitationBooleans(bool useGravity, bool isKinematic, bool canBeLevited)
    {
        _rigidbody.useGravity = useGravity;
        _rigidbody.isKinematic = isKinematic;
        CanBeLevitated = canBeLevited;
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
        transform.position = _spawnLocation;
        transform.rotation = _spawnRotation;
        _rigidbody.velocity = Vector3.zero;
    }
}
