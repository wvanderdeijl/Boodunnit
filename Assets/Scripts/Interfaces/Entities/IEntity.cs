using UnityEngine;

namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        void Move(Vector3 direction);
        
        void CheckSurroundings();
        
        void UseFirstAbility();

        Dialogue Dialogue
        {
            get;
        }

        Question Question
        {
            get;
        }

        string CharacterName
        {
            get;
            set;
        }
    }
}