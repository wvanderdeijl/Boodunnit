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
        NavMeshAgent = GetComponent<NavMeshAgent>();

        if (NavMeshAgent)
        {
            NavMeshAgent.autoBraking = true;

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
        }
    }
    private void FollowTarget()
    {
        if (TargetToFollow)
        {
            float distanceToTarget = Vector3.Distance(transform.position, TargetToFollow.transform.position);
            if (distanceToTarget > MinimumFollowRange && distanceToTarget < MaximumFollowRange)
            {
                NavMeshAgent.isStopped = false;
                NavMeshAgent.SetDestination(TargetToFollow.transform.position);
                return;
            }

            NavMeshAgent.isStopped = true;
        }
    }

    private void ReturnToSpawn()
    {
        if (HasReachedDestination(_spawnLocation))
        {
            NavMeshAgent.isStopped = false;
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
            _patrolDestination = EntityAreaHandler.Instance.GetRandomPositionInArea(_currentArea, gameObject);
            NavMeshAgent.destination = _patrolDestination;
            _hasPositionInArea = true;
        }

        if (HasReachedDestination(_patrolDestination))
        {
            _hasPositionInArea = false;
        }
    }

    private bool HasReachedDestination(Vector3 destination)
    {
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        return distanceToDestination < 0.5f;
    }

    public IEnumerator StartCountdownInArea(float amountOfTime)
    {
        yield return new WaitForSeconds(amountOfTime);
        _currentArea = null;
        IsOnCountdown = false;
    }

    private void MoveToNextArea()
    {
        _currentArea = EntityAreaHandler.Instance.GetRandomAreaForEntity(gameObject);
        _hasPositionInArea = false;
        NavMeshAgent.ResetPath();
    }
    
    public void ChangePathFindingState(PathFindingState pathFindingState)
    {
        _pathFindingState = pathFindingState;
    }

    public void ResetDestination()
    {
        NavMeshAgent.destination = _patrolDestination;
    }
}