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
    public string entityName;
    //public Question question;
    public Sentence[] sentences;
}

