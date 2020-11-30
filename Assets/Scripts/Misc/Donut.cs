using System;
using System.Collections;
using System.Numerics;
using DefaultNamespace.Enums;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Donut : MonoBehaviour
{
    public bool IsTargeted { get; set; }
    public LevitationState State { get; set; }
    public GameObject PoliceMan { get; set; }

    private Rigidbody _rigidbody;
    private LevitateableObject _levitateableObject;
    private PoliceManBehaviour _policeManBehaviour;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _levitateableObject = GetComponent<LevitateableObject>();
    }
    
    public IEnumerator MoveToPosition()
    {
        IsTargeted = true;
        _policeManBehaviour = PoliceMan.GetComponent<PoliceManBehaviour>();
        Destroy(_levitateableObject);
        
        transform.position = PoliceMan.transform.position + (PoliceMan.transform.forward * 0.25f) + 
                             new Vector3(0, 
                                 _policeManBehaviour.ConsumableEndPosition.transform.localPosition.y, 0);
        yield return new WaitForSeconds(1.5f);
        while (Vector3.Distance(transform.position, _policeManBehaviour.ConsumableEndPosition.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                _policeManBehaviour.ConsumableEndPosition.position, 0.02f * Time.deltaTime);
            yield return null;
        }
        GetConsumed();
    }

    public void GetConsumed()
    {
        _policeManBehaviour.ResetDestination();
        
        //TODO deze boi moet gedestroyed worden na de refactor van de Highlight- en LevitateBehaviour.
        gameObject.SetActive(false);
    }
}