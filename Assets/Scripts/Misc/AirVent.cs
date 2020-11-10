using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVent : MonoBehaviour
{
    public int AirPower;
    private void OnTriggerStay(Collider other)
    {
        other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * AirPower, ForceMode.VelocityChange);
        /**
        GlideBehaviour glideBehaviour = PossessionBehaviour.PossessionTarget.GetComponent<GlideBehaviour>();
        if (glideBehaviour != null)
        {
            if (glideBehaviour.IsGliding)
            {
                other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * AirPower, ForceMode.VelocityChange);
            }
        }
        **/
    }
}
