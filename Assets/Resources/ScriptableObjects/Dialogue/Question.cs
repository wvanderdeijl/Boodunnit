using UnityEngine;

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class Question : ScriptableObject
{
    [TextArea(2, 5)]
    public string Text;
    public Choice[] Choices;
}