using System;
using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public float Speed;
    public bool IsGrounded = true;

    private float _rotationSpeed = 10f;
    private float _gravity = 9.81f;
    public float JumpForce = 10.0f;
    [SerializeField] private bool _hasCollidedWithWall;
    public Collider Collider;
    private ContactPoint[] _contacts;

    private void LateUpdate()
    {
        foreach (var contact in _contacts)
        {
            
            Vector3 contactDirection = (contact.point - transform.position);
            Debug.DrawRay(transform.position, contactDirection, Color.red);
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
                        print("angle: " + contactAngle +  "direction: " + direction);
                    }    
                    else if (Math.Abs(contactAngle) > 90)
                    {
                        print(contactAngle);
                        continue;
                    }
                    else
                    {
                        direction = Vector3.zero;
                        print("no movement allowed");
                    }
                }
            }
        }
        print("moving");
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
}