using Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [HideInInspector]
    public bool hasDialogueStarted;

    public Text EntityNameUIText;
    public Text DialogueUIText;
    public Animator Animator;

    private Queue<string> _sentences = new Queue<string>();
    private Question _question = null;
    private NPCCharacter _character;

    //This field is created by the lead dev, the way you interact with others must change in the future, the code on line 25 will act weird if there are more than 1 entity to talk to in range!, for now this will do. No more weird interaction point sphere stuff also please.
    private float _dialogTriggerRadius = 5;//ToDo: Remove this line later when interaction with the world is thought about by the lead dev and lead game designer.

    public void TriggerDialogue(bool isPossesing)//ToDo; The way the world is interacted with will change, this means that line 25 will not do, this method will cause weird behaviour when more than 1 talkable entity is in range anyway.
    {
        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, _dialogTriggerRadius);
        foreach (Collider entityCollider in hitColliderArray)
        {
            if (entityCollider.TryGetComponent(out IHuman humanToTalkTo) && isPossesing)
            {
                hasDialogueStarted = true;
                _character = humanToTalkTo.Character;
                EntityNameUIText.text = $"{_character}";

                ManageDialogue(humanToTalkTo.Dialogue, humanToTalkTo.Question);

                break;
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
        DialogueUIText.text = question.Text.ToString();

        foreach (Choice choice in question.Choices)
        {
            Button buttonInstance = ButtonPooler.Instance.SpawnFromPool("ChoiceButton", Vector3.zero, Quaternion.identity, true, choice.Text.ToString());

            buttonInstance.onClick.AddListener(delegate () { ManageDialogue(choice.Dialogue, choice.Question); });

            //if entiry proffesion does not match disable button interaction
            if (_character != choice.ProffesionUnlocksChoice)
            {
                buttonInstance.interactable = false;
            }
        }
    }

    private void ResetQuestions()
    {
        int poolSize = FindObjectOfType<ButtonPooler>().poolSize;

        //Reset all buttons to orignial state if not used or there is a next question
        for (int i = 0; i < poolSize; i++)
        {
            ButtonPooler.Instance.SpawnFromPool("ChoiceButton", Vector3.zero, Quaternion.identity, false, " ");
        }
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            if (_question)
            {
                ManageDialogue(null, _question);
                return;
            }

            ManageDialogue(null, null);
        } 
        else
        {
            string sentence = _sentences.Dequeue();

            StopAllCoroutines();

            //To-do: Add variable with typespeed from settings
            StartCoroutine(TypeSentence(sentence, 0));
        }
    }

    IEnumerator TypeSentence(string sentence, int typespeed)
    {
        DialogueUIText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            DialogueUIText.text += letter;
            yield return new WaitForSeconds(typespeed);
        }
    }

    private void EndConversation()
    {
        Animator.SetBool("IsOpen", false);
        hasDialogueStarted = false;
    }

    private void OnDrawGizmosSelected()
    {
         Gizmos.DrawWireSphere(transform.position, _dialogTriggerRadius);
    }
}
