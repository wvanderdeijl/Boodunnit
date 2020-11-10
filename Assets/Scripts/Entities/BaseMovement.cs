using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public float Speed;

    private float _rotationSpeed = 10f;

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
}