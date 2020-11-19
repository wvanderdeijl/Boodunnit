using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;

namespace Entities
{
    public class Dog : MonoBehaviour, IAnimal
    {
        [Header("Conversation Settings")]
        public bool DogCanTalkToBoolia;
        public CharacterList DogName;
        public Dialogue DogDialogue;
        public Question DogQuestion;
        public List<CharacterList> DogRelationships;

        [Header("Default Dialogue Answers")]
        public Sentence[] DefaultAnswersList;

        public bool CanTalkToBoolia
        {
            get { return DogCanTalkToBoolia; }
            set => DogCanTalkToBoolia = value;
        }
        public CharacterList CharacterName
        {
            get { return DogName; }
            set => DogName = value;
        }
        public Dialogue Dialogue
        {
            get { return DogDialogue; }
            set => DogDialogue = value;
        }
        public Question Question
        {
            get { return DogQuestion; }
            set => DogQuestion = value;
        }
        public List<CharacterList> Relationships
        {
            get { return DogRelationships; }
            set => DogRelationships = value;
        }
        public Sentence[] DefaultAnswers
        {
            get { return DefaultAnswersList; }
            set => DefaultAnswersList = value;
        }

        public bool IsPossessed { get; set; }
        public float FearThreshold { get; set; }
        public float FearDamage { get; set; }
        public float FaintDuration { get; set; }
        public EmotionalState EmotionalState { get; set; }
        public Dictionary<Type, float> ScaredOfGameObjects { get; set; }
        public void DealFearDamage(float amount)
        {
            throw new NotImplementedException();
        }

        public IEnumerator CalmDown()
        {
            throw new System.NotImplementedException();
        }

        public void Faint()
        {
            throw new System.NotImplementedException();
        }

        public void Move(Vector3 direction)
        {
            throw new NotImplementedException();
        }

        public void EntityJump()
        {
            throw new NotImplementedException();
        }

        public void CheckSurroundings()
        {
            throw new System.NotImplementedException();
        }

        public void UseFirstAbility()
        {
            throw new System.NotImplementedException();
        }

        public void UseSecondAbility()
        {
            throw new System.NotImplementedException();
        }

        public IEntity GetBehaviour()
        {
            return this;
        }
    }
}