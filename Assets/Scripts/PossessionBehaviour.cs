using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PossessionBehaviour : MonoBehaviour
{
    public MeshRenderer PlayerMesh;
    public bool IsPossessing = false;

    private float _possessionRadius = 1;
    private GameObject _possessionTarget;

    public void LeavePossessedTarget()
    {
        if (_possessionTarget && IsPossessing)
        {
            IsPossessing = false;
            transform.position = _possessionTarget.transform.position;
            PlayerMesh.enabled = true;
            _possessionTarget = null;
        }
    }

    public void PossessTarget()
    {
        if (IsPossessing)
        {
            return;
        }

        if (Physics.SphereCast(transform.position, _possessionRadius, transform.forward, out RaycastHit raycastHit, 1))
        {
            GameObject possessionGameObject = raycastHit.transform.gameObject;
            IPossessable possessableInterface = possessionGameObject.GetComponent<IPossessable>();
            if (possessionGameObject && possessableInterface != null && !_possessionTarget)
            {
                _possessionTarget = possessionGameObject;
                IsPossessing = true;
                transform.position = _possessionTarget.transform.position;
                PlayerMesh.enabled = false;
            }
        } 
        else if (_possessionTarget)
        {
            _possessionTarget = null;
        }
    }
}
