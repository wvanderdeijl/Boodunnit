using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string Text;
    public Dialogue dialogue;
    public Question question;
}

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    [TextArea(2, 5)]
    public string Text;
    public enum Conversation { Question, Dialogue }
    public Choice[] choices;
}