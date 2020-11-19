using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class BirdBehaviour : BaseMovement, IEntity, IPossessable
{
    public bool IsPossessed { get; set; }
    public Mesh NotGlidingMesh, GlidingMesh;
    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }

    public Dialogue Dialogue => throw new NotImplementedException();

    public Question Question => throw new NotImplementedException();

    public CharacterList CharacterName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<CharacterList> Relationships => throw new NotImplementedException();

    public Sentence[] DefaultAnswers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField] private GlideBehaviour _glideBehaviour;
    
    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!IsPossessed)
        {
            Rigidbody.isKinematic = true;
            MoveWithPathFinding();
        }
        else
        {
            Rigidbody.isKinematic = false;
        }
    }

    public void DealFearDamage(float amount)
    {
        throw new NotImplementedException();
    }

    public IEnumerator CalmDown()
    {
        throw new NotImplementedException();
    }

    public void Faint()
    {
        throw new NotImplementedException();
    }

    public void Move(Vector3 direction)
    {
        if(_glideBehaviour.IsGliding) MoveEntityInDirection(direction, Speed / 1.5f);
        else MoveEntityInDirection(direction);
    }

    public void EntityJump()
    {
        //Jump
        if (IsGrounded)
        {
            Jump();
        }
    }

    public void CheckSurroundings()
    {
        throw new NotImplementedException();
    }

    public void UseFirstAbility()
    {
        _glideBehaviour.ToggleGlide();

        if (_glideBehaviour.IsGliding)
        {
            GetComponent<MeshFilter>().mesh = GlidingMesh;
        }
        else
        {
            GetComponent<MeshFilter>().mesh = NotGlidingMesh;
        }
    }
}
