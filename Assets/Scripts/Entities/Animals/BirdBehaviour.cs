using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;

public class BirdBehaviour : BaseMovement, IEntity, IPossessable
{
    public Mesh NotGlidingMesh, GlidingMesh;

    public float FearThreshold { get; set; }
    public float FearDamage { get; set; }
    public float FaintDuration { get; set; }
    public EmotionalState EmotionalState { get; set; }
    public Dictionary<Type, float> ScaredOfGameObjects { get; set; }

    public Dialogue Dialogue => throw new NotImplementedException();

    public Question Question => throw new NotImplementedException();

    public string CharacterName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField] private GlideBehaviour _glideBehaviour;
    
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
        if(_glideBehaviour.IsGliding) MoveEntityInDirection(direction, Speed / 2f);
        else MoveEntityInDirection(direction);
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
