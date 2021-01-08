using Enums;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string Text;
    public List<CharacterType> CharacterUnlocksChoice;
    public string ClueUnlocksChoice;
    public int IndexToReplace;
    public bool Show;
    public Dialogue Dialogue;
    public Question Question;
}