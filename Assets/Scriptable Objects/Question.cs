using UnityEngine;

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string choiceText;
    public Dialogue dialogue;
}

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    [TextArea(2, 5)]
    public string question;
    public Choice[] choices;
}