using UnityEngine;

public class turn : MonoBehaviour
{
    float smooth = 1f;
    float tiltAngle = 90f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the cube by converting the angles into a quaternion.
        Quaternion target = Quaternion.Euler(90f, 0, 0);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
    }
}
