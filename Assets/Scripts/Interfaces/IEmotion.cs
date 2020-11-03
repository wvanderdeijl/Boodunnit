using System;
using System.Collections;
using System.Collections.Generic;
using Enums;

namespace Interfaces
{
    public interface IEmotion
    {
        float FearThreshold { get; set; }
        float FearDamage { get; set; }
        float FaintDuration { get; set; }
        EmotionalState EmotionalState { get; set; }
        Dictionary<Type, float> ScaredOfGameObjects { get; set; }

        void DealFearDamage(float amount);

        IEnumerator CalmDown();

        void Faint();
    }
}