using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public float Speed;
    public bool IsGrounded = true;

    private float _rotationSpeed = 10f;
    private float _gravity = 9.81f;
    private float _jumpForce = 10.0f;

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
}