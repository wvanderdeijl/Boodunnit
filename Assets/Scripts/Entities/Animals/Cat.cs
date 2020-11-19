using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class Cat : MonoBehaviour, IAnimal
    {
        [Header("Conversation Settings")]
        public bool CatCanTalkToBoolia;
        public CharacterList CatName;
        public Dialogue CatDialogue;
        public Question CatQuestion;
        public List<CharacterList> CatRelationships;

        [Header("Default Dialogue Answers")]
        public Sentence[] DefaultAnswersList;

        public bool CanTalkToBoolia
        {
            get { return CatCanTalkToBoolia; }
            set => CatCanTalkToBoolia = value;
        }
        public CharacterList CharacterName
        {
            get { return CatName; }
            set => CatName = value;
        }
        public Dialogue Dialogue
        {
            get { return CatDialogue; }
            set => CatDialogue = value;
        }
        public Question Question
        {
            get { return CatQuestion; }
            set => CatQuestion = value;
        }
        public List<CharacterList> Relationships
        {
            get { return CatRelationships; }
            set => CatRelationships = value;
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

        private RagdollControler _ragdollControler;

        [SerializeField] private float _radius;
        [SerializeField] private float _angle;
        [SerializeField] private Image _fearMeter;
        
        private bool _hasFearCooldown;
        
        private void Awake()
        {
            _ragdollControler = GetComponent<RagdollControler>();
            
            FearThreshold = 20;
            FearDamage = 0;
            FaintDuration = 10;
            EmotionalState = EmotionalState.Calm;
            ScaredOfGameObjects = new Dictionary<Type, float>()
            {
                [typeof(Dog)] = 3f,
                [typeof(IHuman)] = 2f,
                [typeof(ILevitateable)] = 5f
            };
            
            _hasFearCooldown = false;
        }

        private void Update()
        {
            CheckSurroundings();
        }

        public void Move(Vector3 direction)
        {
            throw new NotImplementedException();
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

        public void DealFearDamage(float fearDamage)
        {
            if (EmotionalState == EmotionalState.Fainted) return;
            
            FearDamage += fearDamage;
            UpdateFearMeter();
            if (FearDamage >= FearThreshold) Faint();
        }

        public IEnumerator CalmDown()
        {
            yield return new WaitForSeconds(FaintDuration);
            EmotionalState = EmotionalState.Calm;
            FearDamage = 0;
            _ragdollControler.ToggleRagdoll(false);
        }

        public void Faint()
        {
            EmotionalState = EmotionalState.Fainted;
            _ragdollControler.ToggleRagdoll(true);
            StartCoroutine(CalmDown());
        }

        public void UseFirstAbility()
        {
            //first ability.
        }

        public void UseSecondAbility()
        {
            //second ability.
        }

        public void UpdateFearMeter()
        {
            _fearMeter.fillAmount = FearDamage / FearThreshold;
            //_fearMeter.fillAmount = Mathf.MoveTowards(_fearMeter.fillAmount, FearDamage, 1);
        }

        public IEntity GetBehaviour()
        {
            return this;
        }
    }
}