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
        
        if (Input.GetMouseButtonDown(0))
        {
            _levitateBehaviour.LevitationStateHandler();
        }
        
        if (Input.GetMouseButton(1))
        {
            _levitateBehaviour.RotateLevitateableObject();
        }

        _levitateBehaviour.PushOrPullLevitateableObject();
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
