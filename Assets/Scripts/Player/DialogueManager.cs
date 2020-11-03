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
    public QuestionManager QuestionManager;

    private Queue<string> _sentences;
    private Question _question;

    private void Start()
    {
        _sentences = new Queue<string>();
    }

    public void TriggerDialogue(Transform radiusPoint, float radius)
    {
        Collider[] hitColliderArray = Physics.OverlapSphere(radiusPoint.position, radius);

        foreach (Collider entityCollider in hitColliderArray)
        {
            GameObject entityGameobject = entityCollider.gameObject;

            if (entityGameobject.TryGetComponent(out IHuman human))
            {
                //Start dialog with entity
                hasDialogueStarted = true;
                StartDialogue(human.Dialogue);
            }
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Animator.SetBool("IsOpen", true);
        _question = dialogue.question;

        EntityName.text = dialogue.entityName;

        _sentences.Clear();

        //Add all sentences to Queue -> FIFO
        foreach (Sentence sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence.Text.ToString());
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            if (_question)
            {
                QuestionManager.StartQuestion(_question, DialogueText);
            } 
            else
            {
                print("we dont have a question");
                EndDialogue();
            }
        } else
        {
            string sentence = _sentences.Dequeue();

            //Stop typing sentence before starting new coroutine
            StopAllCoroutines();

            StartCoroutine(TypeSentence(sentence, 0));
        }
    }

    IEnumerator TypeSentence(string sentence, float _typeSpeed)
    {
        DialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(_typeSpeed);
        }
    }

    public void EndDialogue()
    {
        Animator.SetBool("IsOpen", false);
        hasDialogueStarted = false;
    }
}
