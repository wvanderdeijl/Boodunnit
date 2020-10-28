using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightBehaviour : MonoBehaviour
{
    public float Radius;

    private Shader _standardShader, _noShader;
    private List<Collider> _previousColliders = new List<Collider>();
    private IEnumerable<Collider> _removeShaderFromColliders;

    private void Awake()
    {
        _standardShader = Shader.Find("Outlined/Highlight");
        _noShader = Shader.Find("Standard");
    }

    public void HighlightTargetsInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Radius);
        List<Collider> currentColliders = new List<Collider>();

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.TryGetComponent(out IPossessable possessable) ||
                hitCollider.gameObject.TryGetComponent(out ILevitatetable levitatetable))
            {
                ChangeShader(hitCollider.gameObject.GetComponent<Renderer>(), _standardShader, true);

                //Add colliders tot the previous list
                if (!_previousColliders.Contains(hitCollider))
                {
                    _previousColliders.Add(hitCollider);
                }

                if (!currentColliders.Contains(hitCollider))
                {
                    currentColliders.Add(hitCollider);
                }
            } 
        }

        //Check differences in previous and current collider list
        _removeShaderFromColliders = _previousColliders.Except(currentColliders);

        foreach (Collider c in _removeShaderFromColliders)
        {
            ChangeShader(c.gameObject.GetComponent<Renderer>(), _noShader, false);
        }
    }

    private void ChangeShader(Renderer renderer, Shader shader, bool addShader)
    {
        if (addShader)
        {
            renderer.material.shader = shader;
        }
        else
        {
            renderer.material.shader = _noShader;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
