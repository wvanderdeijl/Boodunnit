using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

public class Donut : MonoBehaviour
{
    public bool IsTargeted { get; set; }
    public bool CanBeLevitated { get; set; }
    public bool CanRespawnWhenOutOfRange { get; set; }
    public bool IsInsideSphere { get; set; }
    public LevitationState State { get; set; }

    private Rigidbody _rigidbody;
    private GameObject _policeMan;

    public IEnumerator LevitateForSeconds(float seconds)
    {
        CanBeLevitated = false;
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        yield return new WaitForSeconds(5f);
        CanBeLevitated = true;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
    }

    public IEnumerator MoveToPosition(GameObject policeMan, Vector3 position)
    {
        IsTargeted = true;
        _policeMan = policeMan;
        CanBeLevitated = false;
        Vector3 lerpToPos = Vector3.Lerp(transform.position, position, 0.25f * Time.deltaTime);
        transform.position = lerpToPos;
        yield return new WaitForSeconds(2f);
        GetConsumed();
    }

    public void GetConsumed()
    {
        Vector3 lerpToPos = Vector3.Lerp(transform.position, transform.position + -_policeMan.transform.forward, 
            0.125f * Time.deltaTime);
        transform.position = lerpToPos;
        Destroy(gameObject, 5f);
    }
}
