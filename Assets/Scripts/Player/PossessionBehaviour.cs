using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossessionBehaviour : MonoBehaviour
{
    public MeshRenderer PlayerMesh;
    public bool IsPossessing = false;

    public Image CooldownImage;

    private float _possessionRadius = 1;
    private GameObject _possessionTarget;

    private float _cooldown = 2f;
    private bool _isOnCooldown = false;

    public void LeavePossessedTarget()
    {
        if (_possessionTarget && IsPossessing)
        {
            IsPossessing = false;
            transform.position = _possessionTarget.transform.position;
            PlayerMesh.enabled = true;
            _possessionTarget = null;
            _isOnCooldown = true;

            StartCoroutine(PossessionTimer());
        }
    }

    public void PossessTarget()
    {
        if (IsPossessing || _isOnCooldown)
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

    private IEnumerator PossessionTimer()
    {
        float currentTime = 0;

        while (currentTime < _cooldown)
        {
            yield return new WaitForEndOfFrame();
            currentTime += Time.deltaTime;
            CooldownImage.fillAmount = currentTime / _cooldown;
        }
        _isOnCooldown = false;
    }
}
