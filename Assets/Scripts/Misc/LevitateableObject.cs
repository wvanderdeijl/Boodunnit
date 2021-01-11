using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

public class LevitateableObject : MonoBehaviour, ILevitateable
{
    [SerializeField] private bool _canRespawnWhenOutOfRange;
    private Vector3 _spawnLocation;
    private Quaternion _spawnRotation;
    private Rigidbody _rigidbody;
    private Outline _outline;
    private GameObject _player;
    
    public LevitationState State { get; set; }
    public int TimesLevitated { get; set; }
    
    public float DespawnDistance = 10f;
    public bool WillLogPossessCount;
    public float MaxDistanceToPlayerWhileFrozen;

    private void Awake()
    {
        State = LevitationState.NotLevitating;
        _rigidbody = GetComponent<Rigidbody>();
        _outline = gameObject.AddComponent<Outline>();
        _player = FindObjectOfType<PlayerBehaviour>().gameObject;
        
        AddOutline();
        SetSpawnLocationAndRotation();
        SetMaxDistanceToPlayerWhileFrozenValue();
        StartCoroutine(CheckForDistance());
    }

    private void Update()
    {
        float distance = Vector3.Distance(gameObject.transform.position, _player.transform.position);
        ChangeOutlineWidthWithDistance(distance);
        if (State != LevitationState.Frozen) return;
        if (distance > MaxDistanceToPlayerWhileFrozen) Release();
    }

    public void Freeze()
    {
        ChangeLayerMask(0);
        ToggleIsKinematic(true);
        State = LevitationState.Frozen;
    }

    public void Release()
    {
        ToggleIsKinematic(false);
        State = LevitationState.NotLevitating;
    }

    public void ToggleIsKinematic(bool isKinematic)
    {
        _rigidbody.useGravity = !isKinematic;
        _rigidbody.isKinematic = isKinematic;
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
        MaxDistanceToPlayerWhileFrozen = levitateBehaviour.CurrentLevitateRadius + 0.1f;
    }

    private void SetSpawnLocationAndRotation()
    {
        _spawnLocation = transform.position;
        _spawnRotation = transform.rotation;
    }

    private void AddOutline()
    {
        if (_outline)
        {
            Color purple;
            ColorUtility.TryParseHtmlString("#6F4F61", out purple);

            _outline.OutlineColor = purple;
            _outline.OutlineMode = Outline.Mode.OutlineVisible;
            _outline.OutlineWidth = 0.0f;
            _outline.enabled = false;
        }
    }

    private void ChangeOutlineWidthWithDistance(float distance)
    {
        _outline.OutlineWidth = 10f * (1f - (distance / (MaxDistanceToPlayerWhileFrozen - 0.1f)));
    }
    
    private IEnumerator CheckForDistance()
    {
        yield return new WaitForSeconds(2f);
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
}
