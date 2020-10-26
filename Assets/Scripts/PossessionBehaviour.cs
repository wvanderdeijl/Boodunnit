using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionBehaviour : MonoBehaviour
{
    private Shader _highlightShader;
    private Shader _standardShader;

    // Start is called before the first frame update
    void Start()
    {
        _highlightShader = Shader.Find("Outlined/Highlight");
        _standardShader = Shader.Find("Standard");
    }

    // Update is called once per frame
    void Update()
    {
        HightlightTargetsInRadius();
    }

    private void HightlightTargetsInRadius()
    {
        Collider[] collides = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider collider in collides)
        {
            GameObject possessableGameObject = collider.gameObject;
            if (possessableGameObject.TryGetComponent<IPossessable>(out IPossessable possessable))
            {
                ChangeShader(possessableGameObject.GetComponent<Renderer>(), _highlightShader);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print(other);
        GameObject possessableGameObject = other.gameObject;
        if (possessableGameObject.TryGetComponent<IPossessable>(out IPossessable possessable))
        {
            ChangeShader(possessableGameObject.GetComponent<Renderer>(), _standardShader);
        }
    }

    private void ChangeShader(Renderer possessableRenderer, Shader shader)
    {
        possessableRenderer.material.shader = shader;
    }
}
