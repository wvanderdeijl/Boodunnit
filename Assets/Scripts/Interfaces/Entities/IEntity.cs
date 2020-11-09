using UnityEngine;

namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        //Dialogue Dialogue
        //{
        //    get;
        //}

        void Move(Vector3 direction);
        
        void CheckSurroundings();
        
        void UseFirstAbility();
    }
}