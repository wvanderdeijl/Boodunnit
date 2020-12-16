using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Enums;
using Enums;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Entities
{
    public abstract class BaseEntity : BaseEntityMovement, IPossessable
    {
        //Public properties
        public bool IsPossessed { get; set; }
        public bool CanPossess = true;
        public bool IsWalking { get; set; }

        public List<AudioClip> WalkAudioClips;

        //Properties & Fields regarding Dialogue mechanic.
        [Header("Conversation")]
        public Dialogue Dialogue;
        public Question Question;
        public List<CharacterType> Relationships;
        public Sentence[] DefaultAnswers;
        public CharacterType CharacterName;
        public bool CanTalkToBoolia;

        [Header("Fear")]
        public float FearThreshold;
        public float FearDamage;
        public float FaintDuration;
        public EmotionalState EmotionalState;
        public Dictionary<CharacterType, float> ScaredOfEntities;
        public bool IsScaredOfLevitatableObject;
        public float LevitatableObjectFearDamage = 10;
        public bool HasFearCooldown;

        [SerializeField] private float _fearRadius;
        [SerializeField] private float _fearAngle;

        [SerializeField] private RagdollController _ragdollController;
        [SerializeField] private Animator _animator;

        protected void InitBaseEntity()
        {
            InitEntityMovement();
            InitVolumeForEntity();

            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            Outline outline = gameObject.AddComponent<Outline>();
            if (outline)
            {
                Color possesionColor;
                ColorUtility.TryParseHtmlString("#ffed85", out possesionColor);
                outline.OutlineColor = possesionColor;
                outline.OutlineMode = Outline.Mode.OutlineVisible;
                outline.OutlineWidth = 5.0f;
                outline.enabled = false;
            }
        }

        private void Update()
        {
            EntityWalkAnimation();
            Rigidbody.isKinematic = !IsPossessed;
            if (!IsPossessed)
            {
                CheckSurroundings();
                if (EmotionalState != EmotionalState.Fainted && FearDamage <= 0)
                {
                    if (_pathFindingState.Equals(PathFindingState.Following) || _pathFindingState.Equals(PathFindingState.PatrolAreas) || _pathFindingState.Equals(PathFindingState.Patrolling))
                        CheckWhichAudioClipToPlayForEntity();

                    MoveWithPathFinding();
                }
                
            }
            IsWalking = (IsGrounded && Rigidbody.velocity != Vector3.zero);
            if (_animator)
            {
                _animator.SetBool("IsWalking", IsWalking);
                _animator.SetBool("IsGrounded", IsGrounded);
            }
        }

        public abstract void UseFirstAbility();

        public void PlayAudioClip(int index)
        {
            if (WalkAudioClips.Count <= 0)
                return;

            AudioSource audio = GetComponent<AudioSource>();
            if (audio)
            {
                audio.clip = WalkAudioClips[index];
                if(!audio.isPlaying)
                    audio.Play();
            }
        }

        public void PlayAudioOnMovement(int index)
        {
            if (IsPossessed)
            {
                if (Rigidbody.velocity.magnitude > 0.01f)
                    PlayAudioClip(index);
                else
                    StopAudioClip();
            }

            if (NavMeshAgent)
            {
                if (NavMeshAgent.velocity.magnitude > 0.01f)
                    PlayAudioClip(index);
                else
                    StopAudioClip();
            }
        }

        public void StopAudioClip()
        {
            AudioSource audio = GetComponent<AudioSource>();
            if (audio)
                audio.Stop();
        }

        public void PlayWalkAudioClip()
        {
            if (WalkAudioClips.Count <= 0)
                return;

            AudioSource audio = GetComponent<AudioSource>();
            int randomWalkAudioClip = UnityEngine.Random.Range(0, WalkAudioClips.Count);
            if (audio)
            {
                audio.clip = WalkAudioClips[randomWalkAudioClip];
                audio.Play();
            }
        }

        protected virtual void CheckSurroundings(Vector3 raycastStartPosition)
        {
            if (HasFearCooldown || EmotionalState == EmotionalState.Fainted || IsPossessed) return;
            StartCoroutine(ActivateCooldown());
            
            Collider[] colliders = Physics.OverlapSphere(raycastStartPosition, _fearRadius);

            List<BaseEntity> baseEntities = colliders
                .Where(c =>
                    !c.isTrigger &&
                    Vector3.Dot((c.transform.root.position - transform.position).normalized, transform.forward) * 100f >= (90f - (_fearAngle / 2f)) &&
                    c.GetComponent<BaseEntity>() &&
                    ScaredOfEntities.ContainsKey(c.GetComponent<BaseEntity>().CharacterName))
                .Select(e => e.GetComponent<BaseEntity>())
                .ToList();

            List<LevitateableObject> levitateables = colliders
                .Where(c =>
                    Vector3.Dot((c.transform.root.position - transform.position).normalized, transform.forward) * 100f >= (90f - (_fearAngle / 2f)) &&
                    c.GetComponent<LevitateableObject>()
                    && c.GetComponent<LevitateableObject>().State != LevitationState.NotLevitating &&
                    IsScaredOfLevitatableObject)
                .Select(l => l.GetComponent<LevitateableObject>())
                .ToList();

            if (baseEntities.Count == 0 && levitateables.Count == 0) CalmDown();
            else
            {
                foreach (BaseEntity entity in baseEntities) if(!IsPossessed) DealFearDamage(ScaredOfEntities[entity.CharacterName]);
                foreach (LevitateableObject levitateable in levitateables) if(!IsPossessed) DealFearDamage(LevitatableObjectFearDamage);
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

            SetScaredStage(FearDamage >= FearThreshold / 2 && EmotionalState != EmotionalState.Fainted ? 2 : 1);
            PauseEntityNavAgent(true);

            if (FearDamage >= FearThreshold && EmotionalState != EmotionalState.Fainted) Faint();
        }

        protected virtual void Faint()
        {
            EmotionalState = EmotionalState.Fainted;
            if (_ragdollController) _ragdollController.ToggleRagdoll(true);
            CanPossess = false;
            StartCoroutine(WakeUp());
        }

        protected virtual void CalmDown()
        {
            if (FearDamage > 0) FearDamage -= FearThreshold / 20f;
            if (FearDamage <= 0)
            {
                if (Animator && Animator.runtimeAnimatorController != null)
                {
                    if (Animator.GetInteger("ScaredStage") > 0 && EmotionalState != EmotionalState.Fainted)
                    {
                        if(Animator.GetCurrentAnimatorStateInfo(0).IsTag("Terrified") || Animator.GetCurrentAnimatorStateInfo(0).IsTag("Scared"))
                        {
                            SetScaredStage(0);
                            Animator.Rebind();
                            ResetDestination();
                        }
                    }
                }
                FearDamage = 0;
            }
        }

        private void EntityWalkAnimation()
        {
            if (IsPossessed)
            {
                if (Rigidbody.velocity.magnitude > 0.01)
                {
                    SetWalkingAnimation(true);
                }
                else
                {
                    SetWalkingAnimation(false);
                }
            } else
            {
                if (NavMeshAgent)
                {
                    if (NavMeshAgent.velocity.magnitude > 0.01)
                    {
                        SetWalkingAnimation(true);
                    }
                    else
                    {

                        SetWalkingAnimation(false);
                    }
                }
            }
        }

        private void InitVolumeForEntity()
        {
            PlayerSettings settings = SaveHandler.Instance.LoadDataContainer<PlayerSettings>();
            if(settings != null)
            {
                AudioSource source = GetComponent<AudioSource>();
                if (source)
                {
                    source.volume = ((float)settings.AudioVolume / 100);
                }
            }
        }

        public void ResetFearDamage()
        {
            SetScaredStage(0);
            SetWalkingAnimation(false);

            if(Animator != null && Animator.runtimeAnimatorController != null)
                Animator.Rebind();

            FearDamage = 0;
        }

        private void SetWalkingAnimation(bool shouldWalk)
        {
            if (Animator && Animator.runtimeAnimatorController != null)
                Animator.SetBool("IsWalking", shouldWalk);
        }

        private void SetScaredStage(int scaredStage)
        {
            if (Animator && Animator.runtimeAnimatorController != null)
                Animator.SetInteger("ScaredStage", scaredStage);
        }

        protected virtual IEnumerator WakeUp()
        {
            yield return new WaitForSeconds(FaintDuration);
            FearDamage = 0;
            EmotionalState = EmotionalState.Calm;

            if(_ragdollController)  _ragdollController.ToggleRagdoll(false);

            if (Animator && Animator.runtimeAnimatorController != null)
            {
                Animator.SetInteger("ScaredStage", 0);
                Animator.Rebind();
            }

            ResetDestination();
            CanPossess = true;
        }

        private void CheckWhichAudioClipToPlayForEntity()
        {
            //TODO: Bird 
            switch (CharacterName)
            {
                case CharacterType.Rat:
                    PlayAudioOnMovement(0);
                    break;
            }
        }
    }
}