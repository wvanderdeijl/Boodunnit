using UnityEngine;

namespace Entities.Humans
{
    public class EmmieBehaviour : BaseEntity
    {
        public bool IsCrying;

        [SerializeField] private Animator _animator;

        //private Animator _animator;
        
        private void Awake()
        {
            InitBaseEntity();
            _animator = GetComponent<Animator>();
            CanJump = false;
        }

        private void LateUpdate()
        {
            _animator.SetBool("IsCrying", IsCrying);
        }

        public override void UseFirstAbility()
        {
            //Todo throw plant pot on Julia's head.
        }
    }
}