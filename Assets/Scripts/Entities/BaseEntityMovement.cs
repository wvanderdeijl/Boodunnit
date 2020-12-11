using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEntityMovement : BaseMovement
{
    public GameObject TargetToFollow;

    [HideInInspector]
    public NavMeshAgent NavMeshAgent;

    [HideInInspector]
    public Animator Animator;

    
    public bool IsOnCountdown;

    [Header("Pathfinding")]
    [SerializeField] private PathFindingState _pathFindingState;
    public float MinimumFollowRange, MaximumFollowRange;
    private bool _isPathFinding;
    private bool _hasPositionInArea;
    private Quaternion _spawnRotation;
    private Vector3 _spawnLocation;
    private Vector3 _patrolDestination;
    private EntityArea _currentArea;
    
    protected void InitEntityMovement()
    {
        InitBaseMovement();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        if (NavMeshAgent)
        {
            NavMeshAgent.autoBraking = true;
            NavMeshAgent.speed = PathfindingSpeed;
            _spawnRotation = transform.rotation;
            _spawnLocation = transform.position;
        }
    }
    
    public void MoveWithPathFinding()
    { 
        switch (_pathFindingState)
        {
            case PathFindingState.Stationary:
                ReturnToSpawn();
                break;
            case PathFindingState.Patrolling:
                PatrolArea();
                break;
            case PathFindingState.Following:
                FollowTarget();
                break;
            case PathFindingState.None:
                break;
        }
    }
    private void FollowTarget()
    {
        if (TargetToFollow)
        {
            float distanceToTarget = Vector3.Distance(transform.position, TargetToFollow.transform.position);
            if (distanceToTarget > MinimumFollowRange && distanceToTarget < MaximumFollowRange)
            {
                PauseEntityNavAgent(false);

                NavMeshAgent.SetDestination(TargetToFollow.transform.position);
                return;
            }

            PauseEntityNavAgent(true);
        }
    }

    private void ReturnToSpawn()
    {
        if (HasReachedDestination(_spawnLocation))
        {
            PauseEntityNavAgent(false);
            NavMeshAgent.destination = _spawnLocation;
            return;
        }
        
        Quaternion lerpToRotation = Quaternion.Lerp(transform.rotation, _spawnRotation, 
            Time.deltaTime * 5f);
        transform.rotation = lerpToRotation;
    }

    private void PatrolArea()
    {
        if (!_currentArea) MoveToNextArea();
        if (!_hasPositionInArea)
        {
            MoveToNextPosition();
            _hasPositionInArea = true;
        }
        if (HasReachedDestination(_patrolDestination) && !IsOnCountdown)
        {
            IsOnCountdown = true;
            StartCoroutine(StartCountdownInArea(_currentArea.GetEntityTimeInArea(gameObject)));
        }
    }

    private bool HasReachedDestination(Vector3 destination)
    {
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        return distanceToDestination < 1f;
    }

    public IEnumerator StartCountdownInArea(float amountOfTime)
    {
        yield return new WaitForSeconds(amountOfTime);
        _currentArea = null;
        IsOnCountdown = false;
        _hasPositionInArea = false;
    }

    private void MoveToNextArea()
    {
        _currentArea = EntityAreaHandler.Instance.GetRandomAreaForEntity(gameObject);
        _hasPositionInArea = false;
        NavMeshAgent.ResetPath();
    }

    private void MoveToNextPosition()
    {
        bool positionIsFound = false;
        if (_patrolDestination == null)
        {
            _patrolDestination = EntityAreaHandler.Instance.GetRandomPositionInArea(_currentArea, gameObject);
            NavMeshAgent.destination = _patrolDestination;
            return;
        }

        Vector3 previousDestionation = _patrolDestination;
        while (!positionIsFound)
        {
            _patrolDestination = EntityAreaHandler.Instance.GetRandomPositionInArea(_currentArea, gameObject);
            if(Vector3.Distance(previousDestionation, _patrolDestination) > 1f)
            {
                positionIsFound = true;
                NavMeshAgent.destination = _patrolDestination;
            }
        }
    }
    
    public void ChangePathFindingState(PathFindingState pathFindingState)
    {
        _pathFindingState = pathFindingState;
    }

    public void ResetDestination()
    {
        PauseEntityNavAgent(false);
        ChangePathFindingState(PathFindingState.Patrolling);
        _hasPositionInArea = false;
    }

    public void PauseEntityNavAgent(bool shouldPause)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.isStopped = shouldPause;
        }
    }
}