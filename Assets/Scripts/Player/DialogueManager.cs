using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [HideInInspector]
    public bool hasDialogueStarted;

    public Text EntityName;
    public Text DialogueText;
    public Animator Animator;

    private Queue<string> _sentences = new Queue<string>();
    private Question _question = null;

    public void TriggerDialogue(Transform radiusPoint, float radius)
    {
        Collider[] hitColliderArray = Physics.OverlapSphere(radiusPoint.position, radius);

        foreach (Collider entityCollider in hitColliderArray)
        {
            GameObject entityGameobject = entityCollider.gameObject;

            if (entityGameobject.TryGetComponent(out IHuman human))
            {
                hasDialogueStarted = true;
                EntityName.text = human.Name;

                ManageDialogue(human.Dialogue, human.Question);
            }
        }
    }

    private void ManageDialogue(Dialogue dialogue, Question question) 
    {
        Animator.SetBool("IsOpen", true);
        ResetQuestions();

        if (dialogue)
        {
            StartDialogue(dialogue);
        } 
        
        if (question)
        {
            AskQuestion(question);
        }

        if (!dialogue && !question)
        {
            EndConversation();
        }
    }

    private void StartDialogue(Dialogue dialogue)
    {
        _question = dialogue.question;

        _sentences.Clear();

        foreach (Sentence sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence.Text.ToString());
        }

        DisplayNextSentence();
    }

    private void AskQuestion(Question question)
    {
        DialogueText.text = question.Text.ToString();

        foreach (Choice choice in question.choices)
        {
            Button buttonInstance = ButtonPooler.Instance.SpawnFromPool("ChoiceButton", Vector3.zero, Quaternion.identity, true, choice.Text.ToString());

            buttonInstance.onClick.AddListener(delegate () { ManageDialogue(choice.dialogue, choice.question); });
        }
    }

    private void ResetQuestions()
    {
        for (int i = 0; i < 5; i++)
        {
            ButtonPooler.Instance.SpawnFromPool("ChoiceButton", Vector3.zero, Quaternion.identity, false, " ");
        }
    }

    private void DisplayNextSentence()
    {
        if (_sentences.Count == 0 && _question)
        {
            ManageDialogue(null, _question);
            return;
        }

        string sentence = _sentences.Dequeue();

        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence, 0));
    }

    IEnumerator TypeSentence(string sentence, int typespeed)
    {
        DialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(typespeed);
        }
    }

    private void EndConversation()
    {
        Animator.SetBool("IsOpen", false);
        hasDialogueStarted = false;
    }
}
