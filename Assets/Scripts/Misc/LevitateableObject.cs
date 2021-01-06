using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

public class LevitateableObject : MonoBehaviour, ILevitateable
{
    [SerializeField] private bool _canRespawnWhenOutOfRange;
    private Vector3 _spawnLocation;
    private Quaternion _spawnRotation;
    private Rigidbody _rigidbody;

    public float DespawnDistance = 10f;
    public int TimesLevitated { get; set; }
    public bool WillLogPossessCount;
    public float MaxDistanceToPlayerWhileFrozen;

    private void Awake()
    {
        CanBeLevitated = true;
        State = LevitationState.NotLevitating;
        _rigidbody = GetComponent<Rigidbody>();
        SetSpawnLocationAndRotation();
        StartCoroutine(CheckForDistance());
        AddOutline();
        SetMaxDistanceToPlayerWhileFrozenValue();
    }

    public void Freeze()
    {
        ChangeLayerMask(0);
        SetRigidbodyAndLevitationBooleans(false, true, false);
        State = LevitationState.Frozen;
    }

    public void Release()
    {
        ChangeLayerMask(16);
        SetRigidbodyAndLevitationBooleans(true, false, true);
        State = LevitationState.NotLevitating;
    }

    private void SetRigidbodyAndLevitationBooleans(bool useGravity, bool isKinematic, bool canBeLevited)
    {
        _rigidbody.useGravity = useGravity;
        _rigidbody.isKinematic = isKinematic;
        CanBeLevitated = canBeLevited;
    }

    private void ChangeLayerMask(int layerMaskParam)
    {
        foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = layerMaskParam;
        }   
    }

    private void SetMaxDistanceToPlayerWhileFrozenValue()
    {
        LevitateBehaviour levitateBehaviour = FindObjectOfType<LevitateBehaviour>();
        MaxDistanceToPlayerWhileFrozen = levitateBehaviour.OverLapSphereRadiusInUnits + 0.1f;
    }

    private void SetSpawnLocationAndRotation()
    {
        _spawnLocation = transform.position;
        _spawnRotation = transform.rotation;
    }

    private void AddOutline()
    {
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
    
    private IEnumerator CheckForDistance()
    {
        yield return new WaitForSeconds(3f);
        if (CanRespawnWhenOutOfRange && 
            Mathf.Abs(Vector3.Distance(transform.position, _spawnLocation)) >= DespawnDistance && 
            Mathf.Abs(Vector3.Distance(transform.position, CameraController.RotationTarget.position)) >= DespawnDistance && 
            Mathf.Abs(Vector3.Distance(_spawnLocation, CameraController.RotationTarget.position)) >= DespawnDistance)
        {
            Despawn();
        }

        StartCoroutine(CheckForDistance());
    }
    
    private void Despawn()
    {
        transform.position = _spawnLocation;
        transform.rotation = _spawnRotation;
        _rigidbody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (State != LevitationState.Levitating) return;
        float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;
        if(collisionForce > 200f) SoundManager.Instance.PlaySound("Levitate_bump");
    }
    
    public bool CanRespawnWhenOutOfRange
    {
        get => _canRespawnWhenOutOfRange;
        set => _canRespawnWhenOutOfRange = value;
    }
    
    public LevitationState State { get; set; }
    
    public bool CanBeLevitated { get; set; }
}
