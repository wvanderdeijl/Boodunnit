using System;
using Enums;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public float Speed;
    public bool IsGrounded = true;
    public GameObject Target;
    public NavMeshAgent NavMeshAgent;

    private float _rotationSpeed = 10f;
    private float _gravity = 9.81f;
    private float _jumpForce = 10.0f;
    private PathFindingState _pathFindingState = PathFindingState.Stationary;
    private bool _isPathFinding;

    private Quaternion _spawnRotation;
    private Vector3 _spawnLocation;

    private void Start()
    {
        _spawnRotation = transform.rotation;
        _spawnLocation = transform.position;
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
                if(transform.position != _spawnLocation) ReturnToSpawn();
                break;
            case PathFindingState.Patrolling:
                break;
            case PathFindingState.Following:
                if(Target) FollowTarget();
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
        float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
        if (distanceToTarget > 3f && distanceToTarget < 10f)
        {
            NavMeshAgent.isStopped = false;
            NavMeshAgent.SetDestination(Target.transform.position);
            return;
        }
        NavMeshAgent.isStopped = true;
        _pathFindingState = PathFindingState.Stationary;
    }

    private void ReturnToSpawn()
    {
        float distanceToDestination = Vector3.Distance(transform.position, NavMeshAgent.destination);
        if (distanceToDestination > 0.5f)
        {
            NavMeshAgent.isStopped = false;
            NavMeshAgent.destination = _spawnLocation;
            return;
        }
        Quaternion lerpToRotation = Quaternion.Lerp(transform.rotation, _spawnRotation, 
            Time.deltaTime * 5f);
        transform.rotation = lerpToRotation;
    }

    public void ChangeState(PathFindingState pathFindingState)
    {
        _pathFindingState = pathFindingState;
    }
}