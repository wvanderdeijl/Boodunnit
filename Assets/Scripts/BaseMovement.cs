using UnityEngine;

public abstract class BaseMovement : MonoBehaviour
{
    public Rigidbody Rigidbody { get; set; }
    
    public float Speed { get; set; }
    
    public void MoveEntityInDirection(Vector3 direction)
    {
        direction = direction.normalized;
        Rigidbody.velocity = direction * Speed;
    }
}