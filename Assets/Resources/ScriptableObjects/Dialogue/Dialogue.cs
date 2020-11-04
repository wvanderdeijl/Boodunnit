using UnityEngine;

[System.Serializable]
public struct Sentence
{
    [TextArea(3, 10)]
    public string Text;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public Sentence[] sentences;
    public Question question;
}

