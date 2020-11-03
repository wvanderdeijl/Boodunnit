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
    public Animator animator;

    private Queue<string> _sentences;

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
        animator.SetBool("IsOpen", true);

        EntityName.text = dialogue.entityName;

        _sentences.Clear();

        //Add all sentences to Queue -> FIFO
        foreach (Sentence sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence.sentence.ToString());
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = _sentences.Dequeue();

        //Stop typing sentence before starting new coroutine
        StopAllCoroutines();

        StartCoroutine(TypeSentence(sentence, 0));
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
        animator.SetBool("IsOpen", false);
        hasDialogueStarted = false;
    }
}
