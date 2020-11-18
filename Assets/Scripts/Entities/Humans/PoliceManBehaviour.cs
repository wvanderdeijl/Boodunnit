using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;

public class PoliceManBehaviour : MonoBehaviour, IHuman, IPossessable
{
    public CharacterList Name;
    public bool IsPossessed { get; set; }
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }
    public CharacterList CharacterName { get; set; }

    [SerializeField][Range(0, 10)] private float _donutDetectionRadius = 10f;
    [SerializeField][Range(0, 360)] private float _donutDetectionAngle = 90f;

    private void Awake()
    {
        CharacterName = Name;
    }

    private void Update()
    {
        CheckDonutsInSurrounding();
    }

    private void CheckDonutsInSurrounding()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _donutDetectionRadius);
        
        foreach (Collider hitCollider in hitColliders)
        {
            RaycastHit hit;

            Vector3 fromPosition = transform.position;
            Vector3 toPosition = hitCollider.transform.position;
            Vector3 direction = toPosition - fromPosition;
            
            float donutAngle = Vector3.Angle(direction, transform.forward);

            if (Physics.Raycast(fromPosition, direction, out hit, _donutDetectionRadius))
            {
                Donut isDonut = hit.collider.gameObject.GetComponent<Donut>();
                
                if (isDonut)
                {
                    if (donutAngle > -(_donutDetectionAngle / 2) && donutAngle < _donutDetectionAngle / 2)
                    {
                        Debug.Log("I SEE: " + hit.transform.gameObject.name);
                    }
                }
            }
        }
    }


    public void DealFearDamage(float amount)
    {
        
    }

    public IEnumerator CalmDown()
    {
        yield return null;
    }

    public void Faint()
    {
        
    }

    public void Move(Vector3 direction)
    {
        
    }

    public void CheckSurroundings()
    {
        
    }

    public void UseFirstAbility()
    {
        
    }

    public Dialogue Dialogue { get; }
    
    public Question Question { get; }

    public List<CharacterList> Relationships => throw new NotImplementedException();

    public Sentence[] DefaultAnswers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
