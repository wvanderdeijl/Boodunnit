using System;
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

    [SerializeField] private PathFindingState _pathFindingState;
    private float _rotationSpeed = 10f;
    private float _gravity = 9.81f;
    public float JumpForce = 10.0f;
    [SerializeField] private bool _hasCollidedWithWall;
    public Collider Collider;
    private ContactPoint[] _contacts;
    private bool _isPathFinding;
    private Quaternion _spawnRotation;
    private Vector3 _spawnLocation;

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
        if (_hasCollidedWithWall)
        {
            foreach (var contact in _contacts)
            {
                Vector3 contactDirection = (contact.point - transform.position);
                
                bool raycast = Physics.Raycast(transform.position, contactDirection, out RaycastHit hit,
                    Vector3.Distance(transform.position, contact.point) + 0.2f, ~LayerMask.GetMask("Player", "PlayerDash", "Possessable"));
                  if (raycast)
                  {
                    if ((hit.normal.y > 0.75 && contact.point.y < transform.position.y - Collider.bounds.size.y/2)) continue;
                    
                    float contactAngle = Vector3.Angle(IgnoreY(direction), IgnoreY(contactDirection));
                    Vector3 contactCross = Vector3.Cross(IgnoreY(direction), IgnoreY(contactDirection));

                    if (contactCross.y < 0) contactAngle = -contactAngle;
                    
                    
                    if (Math.Abs(contactAngle) > 10 && Math.Abs(contactAngle) <= 90)
                    {
                        float angle = contactAngle > 0 ? 90 : contactAngle < 0 ? -90 : 0;
                        direction = Quaternion.Euler(0, angle, 0) * hit.normal * (Math.Abs(contactAngle)/90);
                    }    
                    else if (Math.Abs(contactAngle) > 90)
                    {
                        print(contactAngle);
                        continue;
                    }
                    else
                    {
                        direction = Vector3.zero;
                    }
                }
            }
        }
        
        float yVelocity = Rigidbody.velocity.y;
        Rigidbody.velocity = direction * speed;
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
                break;
            case PathFindingState.Following:
                FollowTarget();
                break;
        }
    }
    
    public void Jump()
    {
        IsGrounded = false;
        Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
    }

    private void OnCollisionStay(Collision other)
    {
        _contacts = other.contacts;
        _hasCollidedWithWall = !IsGrounded;
    }
    
    private Vector3 IgnoreY(Vector3 input)
    {
        input.y = 0;
        return input;
    }

    private void OnTriggerEnter(Collider other)
    {
        IsGrounded = true;
    }

    private void OnTriggerStay(Collider other)
    {
        IsGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsGrounded = false;
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
        float distanceToDestination = Vector3.Distance(transform.position, _spawnLocation);
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

    public void ChangePathFindingState(PathFindingState pathFindingState)
    {
        _pathFindingState = pathFindingState;
    }
}