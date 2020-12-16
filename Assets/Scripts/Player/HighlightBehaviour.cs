using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class HighlightBehaviour : MonoBehaviour
{
    private Dictionary<string, float> _highlighRadius;
    private float _maxRadius;
    private Vector3 _position;
    private Collider _currentCollider;
    private Collider _previousCollider;

    public Collider HighlightGameobject(Dictionary<string, float> highlightRadius)
    {
        _highlighRadius = highlightRadius;
        _maxRadius = _highlighRadius.Values.Max();

        FindCollidersInFrontOfPlayer();

        return _currentCollider;
    }

    public void FindCollidersInFrontOfPlayer()
    {
        if (PossessionBehaviour.PossessionTarget)
        {
            _position = PossessionBehaviour.PossessionTarget.transform.position;
        } 
        else
        {
            _position = transform.position;
        }

        Collider[] currentObjects = Physics.OverlapSphere(_position, _maxRadius)
            .Where(c => { return IsHighlightable(c) && IsObjectInAngle(c); })
            .ToArray();

        _currentCollider = GetClosestCollider(currentObjects);

        if (_currentCollider != null)
        {
            Outline outline = _currentCollider.gameObject.GetComponent<Outline>();

            if (outline)
            {
                ToggleOutlineScriptOnGameobject(outline, true);
            }
        }

        if (_previousCollider != _currentCollider)
        {
            if (_previousCollider != null)
            {
                Outline outline = _previousCollider.gameObject.GetComponent<Outline>();

                if (outline)
                {
                    ToggleOutlineScriptOnGameobject(outline, false);
                }
            }

            _previousCollider = _currentCollider;
        }
    }

    public bool IsHighlightable(Collider collider)
    {
        IPossessable possessableObject = collider.GetComponent<IPossessable>();
        ILevitateable levitateableObject = collider.GetComponent<ILevitateable>();
        WorldSpaceClue worldSpaceClue = collider.GetComponent<WorldSpaceClue>();

        if (PossessionBehaviour.PossessionTarget)
        {
            return possessableObject != null;
        }
        else
        {
            return possessableObject != null || levitateableObject != null || worldSpaceClue != null;
        }
    }

    public bool IsObjectInAngle(Collider collider)
    {
        Vector3 colliderDirection;
        float angle;

        //if possesing and player is not possesion target
        if (PossessionBehaviour.PossessionTarget && gameObject != PossessionBehaviour.PossessionTarget)
        {
            //check if object is in front of the player
            colliderDirection = (collider.transform.position - PossessionBehaviour.PossessionTarget.transform.position).normalized;
            angle = Vector3.Angle(colliderDirection, PossessionBehaviour.PossessionTarget.transform.forward);
        }
        else
        {
            colliderDirection = (collider.transform.position - transform.position).normalized;
            angle = Vector3.Angle(colliderDirection, transform.forward);
        }

        return angle > -90 && angle < 90;
    }

    public Collider GetClosestCollider(Collider[] colliders)
    {
        float minRadius = _maxRadius;
        Collider closestCollider = null;

        foreach (Collider collider in colliders)
        {
            //if collider is not null
            if (collider)
            {
                float distance = Vector3.Distance(_position, collider.transform.position);

                //check the extra distances distance < minDistance
                if (!PossessionBehaviour.PossessionTarget &&
                    (collider.GetComponent<ILevitateable>() != null && distance < _highlighRadius["LevitateRadius"] ||
                     collider.GetComponent<IPossessable>() != null && distance < _highlighRadius["PossesionRadius"] ||
                     collider.GetComponent<WorldSpaceClue>() != null && distance < _highlighRadius["ClueRadius"]) &&
                     distance < minRadius)
                {
                    minRadius = distance;
                    closestCollider = collider;
                }

                //if possesing
                else if (PossessionBehaviour.PossessionTarget && PossessionBehaviour.PossessionTarget != collider.gameObject)
                {
                    //if collider is Possesable and within radius
                    if (collider.gameObject.GetComponent<IPossessable>() != null && distance < _highlighRadius["ConversationRadius"])
                    {
                        if (distance < minRadius)
                        {
                            minRadius = distance;
                            closestCollider = collider;
                        }
                    }
                }
            }
        }

        return closestCollider;
    }
    private void ToggleOutlineScriptOnGameobject(Outline outline, bool active)
    {
        outline.enabled = active;
    }
}