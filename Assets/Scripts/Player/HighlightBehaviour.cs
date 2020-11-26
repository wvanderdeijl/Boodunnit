using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HighlightBehaviour : MonoBehaviour
{
    public float Radius;

    private List<Collider> _previousColliders = new List<Collider>();
    private IEnumerable<Collider> _removeShaderFromColliders;

    public void HighlightGameobjectsInRadius()
    {
        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, Radius);
        List<Collider> currentCollidersList = new List<Collider>();

        foreach (Collider hitCollider in hitColliderArray)
        {
            if (hitCollider.TryGetComponent(out IPossessable possessable) || hitCollider.TryGetComponent(out ILevitateable levitateable) || hitCollider.TryGetComponent(out WorldSpaceClue worldSpaceClue))
            {

                Outline outline = hitCollider.gameObject.GetComponent<Outline>();

                if (outline)
                {
                    ToggleOutlineScriptOnGameobject(outline, true);
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
            Outline outline = collider.gameObject.GetComponent<Outline>();

            if (outline)
            {
                ToggleOutlineScriptOnGameobject(outline, false);
            }
        }
    }

    private void ToggleOutlineScriptOnGameobject(Outline outline, bool active)
    {
        outline.enabled = active;
    }
}
