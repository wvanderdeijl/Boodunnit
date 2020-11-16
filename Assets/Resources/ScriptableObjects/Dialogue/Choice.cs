using UnityEngine;

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string Text;
    public string CharacterUnlocksChoice;
    public Dialogue Dialogue;
    public Question Question;
}
