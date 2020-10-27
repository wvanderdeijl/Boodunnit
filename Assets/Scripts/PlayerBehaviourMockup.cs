using System.Collections;
using UnityEngine;

public class PlayerBehaviourMockup : MonoBehaviour
{
    [SerializeField] private LevitateBehaviour _levitateBehaviour;
    [SerializeField] private TerrifyBehaviour _terrifyBehaviour;

    private bool _isOnCooldown;

    private void Start()
    {
        _isOnCooldown = false;
    }

    private void Update()
    {
        _levitateBehaviour.ReceiveObjectRigidbodyWithMouseClick();

        if (Input.GetKeyDown(KeyCode.Mouse1) && !_isOnCooldown)
        {
            _terrifyBehaviour.TerrifyEntities();
            StartCoroutine(ActivateCooldown());
        }
    }

    private void FixedUpdate()
    {
        _levitateBehaviour.MoveLevitateableObject();
    }

    private IEnumerator ActivateCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(5f);
        _isOnCooldown = false;
    }
}
