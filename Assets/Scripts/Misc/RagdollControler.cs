using UnityEngine;

public class RagdollControler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Animator _characterAnimator;
    
    private Rigidbody[] _ragdollRigidbodies;
    private Collider[] _ragdollColliders;

    private void Awake()
    {
        InitializeRagdollControler();
    }

    private void InitializeRagdollControler()
    {
        if (GetComponentsInChildren<Rigidbody>() == null) return;
        if (GetComponentsInChildren<Collider>() == null) return;
        
        GetRigidbodiesAndColliders();
        ToggleRagdoll(false);
    }

    private void GetRigidbodiesAndColliders()
    {
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = GetComponentsInChildren<Collider>();
    }

    public void ToggleRagdoll(bool isRagdoll)
    {

        if (_characterAnimator)
        {
            _characterAnimator.enabled = !isRagdoll;
        }
        
        foreach (Rigidbody ragdollRigidbody in _ragdollRigidbodies)
        {
            ragdollRigidbody.isKinematic = !isRagdoll;
        }

        foreach (Collider ragdollCollider in _ragdollColliders)
        {
            ragdollCollider.enabled = isRagdoll;
        }
    }
}
