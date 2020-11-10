using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightBehaviour : MonoBehaviour
{
    public float Radius;

    private Shader _outlineShader, _standardShader;
    private List<Collider> _previousColliders = new List<Collider>();
    private IEnumerable<Collider> _removeShaderFromColliders;

    private void Awake()
    {
        _outlineShader = Shader.Find("Outlined/Highlight");
        _standardShader = Shader.Find("Standard");
    }

    public void HighlightGameobjectsInRadius()
    {
        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, Radius);
        List<Collider> currentCollidersList = new List<Collider>();

        foreach (Collider hitCollider in hitColliderArray)
        {
            if (hitCollider.TryGetComponent(out IPossessable possessable) || hitCollider.TryGetComponent(out ILevitateable levitateable))
            {
                Renderer renderer = hitCollider.GetComponent<Renderer>();

                if (renderer)
                {
                    ChangeShader(renderer, _outlineShader);
                }

                //Add colliders tot the previous list
                if (!_previousColliders.Contains(hitCollider))
                {
                    _previousColliders.Add(hitCollider);
                }

                if (!currentCollidersList.Contains(hitCollider))
                {
                    currentCollidersList.Add(hitCollider);
                }
            } 
        }

        //Check differences in previous and current collider list
        _removeShaderFromColliders = _previousColliders.Except(currentCollidersList);

        foreach (Collider collider in _removeShaderFromColliders)
        {
            Renderer renderer = collider.gameObject.GetComponent<Renderer>();

            if (renderer)
            {
                ChangeShader(renderer, _standardShader);
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
