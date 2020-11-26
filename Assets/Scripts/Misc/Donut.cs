using System;
using System.Collections;
using DefaultNamespace.Enums;
using UnityEngine;

public class Donut : MonoBehaviour
{
    public bool IsTargeted { get; set; }
    public LevitationState State { get; set; }
    public GameObject PoliceMan { get; set; }

    private Rigidbody _rigidbody;
    private LevitateableObject _levitateableObject;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _levitateableObject = GetComponent<LevitateableObject>();
    }

    public IEnumerator MoveToPosition(Vector3 position)
    {
        _rigidbody.isKinematic = true;
        IsTargeted = true;
        _levitateableObject.CanBeLevitated = false;
        // Vector3 moveToPos = Vector3.MoveTowards(transform.position, position, 
        //     0.25f * Time.deltaTime);
        _rigidbody.MovePosition(position);
        yield return new WaitForSeconds(5f);
        GetConsumed();
    }

    public void GetConsumed()
    {
        // Vector3 moveToPos = Vector3.MoveTowards(transform.position, transform.position + -_policeMan.transform.forward, 
        //     0.125f * Time.deltaTime);
        _rigidbody.MovePosition(transform.position + -PoliceMan.transform.forward);
        
        PoliceMan.GetComponent<PoliceManBehaviour>().ResetDestination();
        
        //TODO deze boi moet gedestroyed worden na de refactor van de Highlight- en LevitateBehaviour.
        gameObject.SetActive(false);
    }

    public GameObject GetPoliceMan()
    {
        return PoliceMan;
    }
}
