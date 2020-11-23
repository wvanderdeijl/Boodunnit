using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public abstract class BaseEntity : BaseEntityMovement, IPossessable
    {
        //Property regarding Possession mechanic.
        public bool IsPossessed { get; set; }
        
        //Properties & Fields regarding Dialogue mechanic.
        [Header("Conversation")]
        public CharacterList Name;
        public Dialogue dialogue;
        public Question question;
        public List<CharacterList> relationships;
        
        [Header("Default Answers")]
        public Sentence[] DefaultAnswersList;
        public Dialogue Dialogue { get => dialogue; }
        public Question Question { get => question; }
        public List<CharacterList> Relationships { get => relationships; }
        public Sentence[] DefaultAnswers { get => DefaultAnswersList; set => DefaultAnswersList = value; }
        public CharacterList CharacterName { get => Name; set => Name = value; }
        
        //Properties & Fields regarding Terrify mechanic.
        public float FearThreshold { get; set; }
        public float FearDamage { get; set; }
        public float FaintDuration { get; set; }
        public EmotionalState EmotionalState { get; set; }
        public Dictionary<Type, float> ScaredOfGameObjects { get; set; } 
        public bool HasFearCooldown { get; set; }
        
        [SerializeField] private float _radius;
        [SerializeField] private float _angle;
        [SerializeField] private Image _fearMeter;
        [SerializeField] private RagdollController _ragdollController;

        private void Start()
        {
            Initialize();
            _fearMeter = GetComponentInChildren<Image>();
        }

        public abstract void UseFirstAbility();
        
        protected virtual void CheckSurroundings()
        {
            if (HasFearCooldown) return;
            StartCoroutine(ActivateCooldown());

            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

            foreach (Collider collider in colliders)
            {
                Vector3 offset = (collider.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(offset, transform.forward);

                if (dot * 100f >= (90 - (_angle / 2f)))
                {
                    BaseEntity scaryEntity = collider.gameObject.GetComponent<BaseEntity>();
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
        
        protected virtual IEnumerator ActivateCooldown()
        { 
            HasFearCooldown = true;
            yield return new WaitForSeconds(0.5f);
            HasFearCooldown = false;
        }

        protected virtual void DealFearDamage(float amount)
        {
            if (EmotionalState == EmotionalState.Fainted) return;

            FearDamage += amount;
            UpdateFearMeter();
            if (FearDamage >= FearThreshold) Faint();
        }

        protected virtual IEnumerator CalmDown()
        {
            yield return new WaitForSeconds(FaintDuration);
            EmotionalState = EmotionalState.Calm;
            FearDamage = 0;
            _ragdollController.ToggleRagdoll(false);
        }

        protected virtual void Faint()
        {
            EmotionalState = EmotionalState.Fainted;
            if (_ragdollController) _ragdollController.ToggleRagdoll(true);
            StartCoroutine(CalmDown());
        }
        
        protected virtual void UpdateFearMeter()
        {
            _fearMeter.fillAmount = FearDamage / FearThreshold;
        }
    }
}