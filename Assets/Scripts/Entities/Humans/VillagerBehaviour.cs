using Enums;
using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VillagerBehaviour : BaseMovement, IPossessable, IHuman
{
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }
    public Dialogue Dialogue { get; }
    public Question Question { get; }
    public string Name { get; }
    public bool IsPossessed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public CharacterList CharacterName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<CharacterList> Relationships => throw new NotImplementedException();

    public Sentence[] DefaultAnswers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField] private float _radius;
    [SerializeField] private float _angle;
    [SerializeField] private Image _fearMeter;

    private bool _hasFearCooldown;

    void Awake()
    {
        // Todo give Name and Profession

        FearThreshold = 20;
        FearDamage = 0;
        FaintDuration = 10;
        EmotionalState = EmotionalState.Calm;
        ScaredOfGameObjects = new Dictionary<Type, float>()
        {
            [typeof(RatBehaviour)] = 3f,
            [typeof(ILevitateable)] = 3f
        };
    }

    void Update()
    {
        CheckSurroundings();
    }

    public void CheckSurroundings()
    {
        if (_hasFearCooldown) return;
        StartCoroutine(ActivateCooldown());

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        foreach (Collider collider in colliders)
        {
            Vector3 offset = (collider.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(offset, transform.forward);

            if (dot * 100f >= (90 - (_angle / 2f)))
            {
                IEntity scaryEntity = collider.gameObject.GetComponent<IEntity>();
                if (scaryEntity != null && ScaredOfGameObjects.ContainsKey(scaryEntity.GetType()))
                {
                    DealFearDamage(ScaredOfGameObjects[scaryEntity.GetType()]);
                }

                ILevitateable levitateableObject = collider.gameObject.GetComponent<ILevitateable>();
                if (levitateableObject != null) //TODO check levitateable state
                {
                    DealFearDamage(ScaredOfGameObjects[levitateableObject.GetType()]);
                }
            }
        }
    }

    private IEnumerator ActivateCooldown()
    {
        _hasFearCooldown = true;
        yield return new WaitForSeconds(0.5f);
        _hasFearCooldown = false;
    }

    public IEnumerator CalmDown()
    {
        yield return null;
    }

    public void DealFearDamage(float amount)
    {
        if (EmotionalState == EmotionalState.Fainted) return;

        FearDamage += amount;
        UpdateFearMeter();
        if (FearDamage >= FearThreshold) Faint();
    }
    public void UpdateFearMeter()
    {
        _fearMeter.fillAmount = FearDamage / FearThreshold;
    }

    public void Faint()
    {
        EmotionalState = EmotionalState.Fainted;
        StartCoroutine(CalmDown());
    }

    public void Move(Vector3 direction)
    {
        MoveEntityInDirection(direction);
    }

    public void UseFirstAbility()
    {
    }
}
