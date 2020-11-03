using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public void StartQuestion(Question question, Text dialogueText)
    {
        dialogueText.text = question.Text.ToString();

        foreach (Choice choice in question.choices)
        {
            AddChoiceButton(choice.Text.ToString());
        }
    }

    private void AddChoiceButton(string choiceText)
    {
        ButtonPooler.Instance.SpawnFromPool("ChoiceButton", Vector3.zero , Quaternion.identity, choiceText);
    }
}
