using UnityEngine;

public class HighlightBehaviour : MonoBehaviour
{
    public float Radius;
    private Shader _standardShader;
    private void Update()
    {
        HighlightTargetsInRadius();
    }

    private void Awake()
    {
        _standardShader = Shader.Find("Outlined/Highlight");
    }

    private void HighlightTargetsInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent<IPossessable>(out IPossessable possessable) ||
                hitCollider.gameObject.TryGetComponent<ILevitatetable>(out ILevitatetable levitatetable) ||
                hitCollider.gameObject.TryGetComponent<IScareable>(out IScareable scareable))
            {
                ChangeShader(hitCollider.gameObject.GetComponent<Renderer>(), _standardShader);
            } 
            else
            {
                //change shader back
            }
        }
    }

    private void ChangeShader(Renderer renderer, Shader shader)
    {
        renderer.material.shader = shader;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
