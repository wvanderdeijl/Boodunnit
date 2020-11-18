using Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        bool IsPossessed { get; set; }
        
        void Move(Vector3 direction);

        void EntityJump();
        
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

        CharacterList CharacterName
        {
            get;
            set;
        }

        List<CharacterList> Relationships
        {
            get;
        }

        Sentence[] DefaultAnswers
        {
            get;
            set;
        }
    }
}