using Enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string Text;
    public List<CharacterList> CharacterUnlocksChoice;
    public Dialogue Dialogue;
    public Question Question;
}
