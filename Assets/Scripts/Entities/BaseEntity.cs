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
        
        protected virtual void CheckSurroundings(Vector3 raycastStartPosition)
        {
            if (HasFearCooldown || EmotionalState == EmotionalState.Fainted) return;
            StartCoroutine(ActivateCooldown());
            
            Collider[] colliders = Physics.OverlapSphere(raycastStartPosition, _fearRadius);

            List<BaseEntity> baseEntities = colliders
                .Where(c => 
                    Vector3.Dot((c.transform.root.position - transform.position).normalized, transform.forward) * 100f >= (90f - (_fearAngle / 2f)) &&
                    c.GetComponent<BaseEntity>() != null &&
                    ScaredOfGameObjects.ContainsKey(c.GetComponent<BaseEntity>().GetType()))
                .Select(e => e.GetComponent<BaseEntity>())
                .ToList();

            List<LevitateableObject> levitateables = colliders
                .Where(c => 
                    Vector3.Dot((c.transform.root.position - transform.position).normalized, transform.forward) * 100f >= (90f - (_fearAngle / 2f)) &&
                    c.GetComponent<LevitateableObject>() != null &&
                    ScaredOfGameObjects.ContainsKey(c.GetComponent<LevitateableObject>().GetType()))
                .Select(l => l.GetComponent<LevitateableObject>())
                .ToList();

            if (baseEntities.Count == 0 && levitateables.Count == 0) CalmDown();
            else
            {
                foreach (BaseEntity entity in baseEntities) DealFearDamage(ScaredOfGameObjects[entity.GetType()]);
                foreach (LevitateableObject levitateable in levitateables) DealFearDamage(ScaredOfGameObjects[levitateable.GetType()]);
            }
        }
        
        protected virtual void CheckSurroundings() { CheckSurroundings(transform.position); }
        
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
            if (FearDamage >= FearThreshold) Faint();
            Debug.Log(FearDamage);
        }

        protected virtual void Faint()
        {
            Debug.Log("Fainted.");
            EmotionalState = EmotionalState.Fainted;
            if (_ragdollController) _ragdollController.ToggleRagdoll(true);
            StartCoroutine(WakeUp());
        }
        
        protected virtual void CalmDown()
        {
            Debug.Log(FearDamage);
            if (FearDamage > 0) FearDamage -= FearThreshold / 20f;
            if (FearDamage < 0) FearDamage = 0;
        }

        protected virtual IEnumerator WakeUp()
        {
            yield return new WaitForSeconds(FaintDuration);
            Debug.Log("Awake");
            EmotionalState = EmotionalState.Calm;
            FearDamage = 0;
            Animator.SetBool("IsScared", false);
            _ragdollController.ToggleRagdoll(false);
        }
    }
}