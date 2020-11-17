using UnityEngine;

namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        //Dialogue Dialogue
        //{
        //    get;
        //}

        bool IsPossessed { get; set; }
        
        void Move(Vector3 direction);

        void EntityJump();
        
        void CheckSurroundings();
        
        void UseFirstAbility();
    }
}