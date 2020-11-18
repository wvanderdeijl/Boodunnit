using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMovement : MonoBehaviour
{
    public GameObject Target;
    public NavMeshAgent NavMeshAgent;
    public Rigidbody Rigidbody;

    public float MinimumFollowRange, MaximumFollowRange;
    public float Speed;
    public bool IsGrounded = true;
    public bool IsOnCountdown;

    [SerializeField] private PathFindingState _pathFindingState;
    private float _rotationSpeed = 10f;
    private float _jumpForce = 10.0f;
    private bool _isPathFinding;
    private bool _hasPositionInArea;
    private Quaternion _spawnRotation;
    private Vector3 _spawnLocation;
    private Vector3 _patrolDestination;
    private EntityArea _currentArea;

    private void Start()
    {
        if (NavMeshAgent)
        {
            NavMeshAgent.autoBraking = true;

            _spawnRotation = transform.rotation;
            _spawnLocation = transform.position;
        }
    }

    public void MoveEntityInDirection(Vector3 direction, float speed)
    {
        float yVelocity = Rigidbody.velocity.y;
        Rigidbody.velocity = direction.normalized * speed;
        Rigidbody.velocity += new Vector3(0f, yVelocity, 0f);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(direction.normalized), Time.deltaTime * _rotationSpeed);
        }
    }

    public void MoveEntityInDirection(Vector3 direction)
    {
        MoveEntityInDirection(direction, Speed);
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
    
    public void Jump()
    {
        IsGrounded = false;
        Rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }

    public void CheckIfGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit raycastHitInfo, 1.2f))
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }
    }

    private void FollowTarget()
    {
        if (Target)
        {
            float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            if (distanceToTarget > MinimumFollowRange && distanceToTarget < MaximumFollowRange)
            {
                NavMeshAgent.isStopped = false;
                NavMeshAgent.SetDestination(Target.transform.position);
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

    public void ChangePathFindingState(PathFindingState pathFindingState)
    {
        _pathFindingState = pathFindingState;
    }

    private bool HasReachedDestination(Vector3 destination)
    {
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        return distanceToDestination < 0.5f;
    }

    public IEnumerator StartCountdownInArea(float amountOfTime)
    {
        Debug.Log("Timer start.");
        yield return new WaitForSeconds(amountOfTime);
        Debug.Log("Timer is done!");
        _currentArea = null;
        IsOnCountdown = false;
    }

    private void MoveToNextArea()
    {
        Debug.Log("Going to a new area.");
        _currentArea = EntityAreaHandler.Instance.GetAreaForSpecificEntity(gameObject);
        _hasPositionInArea = false;
        NavMeshAgent.ResetPath();
    }
}