using Enums;
using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IEntity : IEmotion
    {
        bool IsPossessed { get; set; }

        bool CanTalkToBoolia { get; set; }
        
        void Move(Vector3 direction);

        void EntityJump();
        
        void CheckSurroundings();
        
        void UseFirstAbility();

        Dialogue Dialogue
        {
            get;
            set;
        }

        Question Question
        {
            get;
            set;
        }

        CharacterList CharacterName
        {
            get;
            set;
        }

        List<CharacterList> Relationships
        {
            get;
            set;
        }

        Sentence[] DefaultAnswers
        {
            get;
            set;
        }
    }
}