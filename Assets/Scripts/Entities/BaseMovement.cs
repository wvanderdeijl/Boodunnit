using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody;
    public float Speed;

    private float _rotationSpeed = 10f;
    private float _gravity = 9.81f;
    private void FixedUpdate()
    {
        Rigidbody.AddForce(Vector3.down * (_gravity * Rigidbody.mass));//ToDo: Why are you multiplying by .mass???, you can just pass a param to tell this method to ignore mass if you want that?
    }

    public void MoveEntityInDirection(Vector3 direction)
    {
        float yVelocity = Rigidbody.velocity.y;
        Rigidbody.velocity = direction.normalized * Speed;
        Rigidbody.velocity += new Vector3(0f, yVelocity, 0f);

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(direction.normalized), Time.deltaTime * _rotationSpeed);
        }
    }
}