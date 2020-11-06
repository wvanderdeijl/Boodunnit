using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public float Speed;

    private float _rotationSpeed = 10f;
    
    public void MoveEntityInDirection(Vector3 direction)
    {
        Rigidbody.velocity = direction.normalized * Speed;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(direction.normalized), Time.deltaTime * _rotationSpeed);
        }
    }
    private void FixedUpdate()
    {
        Rigidbody.AddForce(Vector3.down * (1000 * Rigidbody.mass));
    }
}