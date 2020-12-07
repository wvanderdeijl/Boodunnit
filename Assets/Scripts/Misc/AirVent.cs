using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVent : MonoBehaviour
{
    public int AirPower;
    private void OnTriggerStay(Collider other)
    {
        GlideBehaviour glideBehaviour = other.gameObject.GetComponent<GlideBehaviour>();
        if (glideBehaviour && glideBehaviour.IsGliding)
        {
            if(other.gameObject == PossessionBehaviour.PossessionTarget)
            {
                Rigidbody otherRigidBody = other.gameObject.GetComponent<Rigidbody>();
                if (otherRigidBody)
                {
                    otherRigidBody.AddForce(transform.forward * AirPower, ForceMode.Acceleration);
                }
            }
        }
    }
}
