using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Enums;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public abstract class BaseEntity : BaseEntityMovement, IPossessable
    {
        //Property regarding Possession mechanic.
        public bool IsPossessed { get; set; }
        public bool CanPossess = true;
        
        //Properties & Fields regarding Dialogue mechanic.
        [Header("Conversation")]
        public Dialogue Dialogue;
        public Question Question;
        public List<CharacterList> Relationships;
        public Sentence[] DefaultAnswers;
        public CharacterList CharacterName;
        public bool CanTalkToBoolia;

        [Header("Fear")]
        public float FearThreshold;
        public float FearDamage;
        public float FaintDuration;
        public EmotionalState EmotionalState;
        public Dictionary<Type, float> ScaredOfGameObjects;
        public bool HasFearCooldown;
        
        [SerializeField] private float _fearRadius;
        [SerializeField] private float _fearAngle;

        [SerializeField] private RagdollController _ragdollController;

        protected void InitBaseEntity()
        {
            InitEntityMovement();

            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Outline outline = gameObject.AddComponent<Outline>();
            if (outline)
            {
                outline.OutlineColor = Color.magenta;
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineWidth = 5.0f;
                outline.enabled = false;
            }
        }

        private void Update()
        {
            Rigidbody.isKinematic = !IsPossessed;
            if (!IsPossessed)
            {
                CheckSurroundings();
                MoveWithPathFinding();
            }
        }

        public abstract void UseFirstAbility();
        
        protected virtual void CheckSurroundings()
        {
            Debug.Log("Checking surroundings...");
            if (HasFearCooldown) return;
            Debug.Log("No fear cooldown.");
            StartCoroutine(ActivateCooldown());

            Collider[] colliders = Physics.OverlapSphere(transform.position, _fearRadius);

            foreach (Collider collider in colliders)
            {
                Vector3 offset = (collider.transform.root.position - transform.position).normalized;
                float dot = Vector3.Dot(offset, transform.forward);

                if (dot * 100f >= (90 - (_fearAngle / 2f)))
                {
                    Debug.Log("Collider in area:");
                    BaseEntity scaryEntity = collider.gameObject.GetComponent<BaseEntity>();
                    if (scaryEntity != null && ScaredOfGameObjects.ContainsKey(scaryEntity.GetType()))
                    {
                        Debug.Log("Dealing fear damage.");
                        DealFearDamage(ScaredOfGameObjects[scaryEntity.GetType()]);
                    }

                    ILevitateable levitateableObject = collider.gameObject.GetComponent<LevitateableObject>();
                    if (levitateableObject != null && (levitateableObject.State == LevitationState.Frozen 
                                                       || levitateableObject.State == LevitationState.Levitating))
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

            Animator.SetInteger("ScaredStage", (FearDamage >= FearThreshold / 2 && EmotionalState != EmotionalState.Fainted) ? 2 : 1);

            if (FearDamage >= FearThreshold) Faint();
        }

        protected virtual void Faint()
        {
            Debug.Log("Fainted.");
            EmotionalState = EmotionalState.Fainted;
            if (_ragdollController) _ragdollController.ToggleRagdoll(true);
            StartCoroutine(CalmDown());
        }
        
        protected virtual IEnumerator CalmDown()
        {
            yield return new WaitForSeconds(FaintDuration);
            EmotionalState = EmotionalState.Calm;
            FearDamage = 0;
            Animator.SetInteger("ScaredStage", 0);
            _ragdollController.ToggleRagdoll(false);
        }
    }
}