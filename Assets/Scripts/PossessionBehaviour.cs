using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PossessionBehaviour : MonoBehaviour
{
    public MeshRenderer PlayerMesh;

    private float _possessionRadius = 0.8f;
    private GameObject _possessionTarget;
    private bool _isPossessing = false;

    // Update is called once per frame
    void Update()
    {
        if (!_isPossessing)
        {
            HighlightPossession();
            PossessTarget();
        } 
        else
        {
            LeavePossessedTarget();
        }
    }

    private void LeavePossessedTarget()
    {
        if (Input.GetKey(KeyCode.E) && _possessionTarget && _isPossessing)
        {
            _isPossessing = false;
            transform.position = _possessionTarget.transform.position;
            PlayerMesh.enabled = true;
            _possessionTarget = null;
        }
    }

    private void PossessTarget()
    {
        if (Input.GetKey(KeyCode.E) && _possessionTarget && !_isPossessing)
        {
            _isPossessing = true;
            transform.position = _possessionTarget.transform.position;
            PlayerMesh.enabled = false;
        }
    }

    private void HighlightPossession()
    {
        if (Physics.SphereCast(transform.position, _possessionRadius, transform.forward, out RaycastHit raycastHit, 1))
        {
            GameObject possessionGameObject = raycastHit.transform.gameObject;

            if (possessionGameObject.TryGetComponent<IPossessable>(out IPossessable possessable) && !_possessionTarget)
            {
                _possessionTarget = possessionGameObject;
            }
        } 
        else if (_possessionTarget)
        {
            _possessionTarget = null;
        }
    }
}
