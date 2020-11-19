﻿using Enums;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    public static bool hasConversationStarted;

    private IEntity _currentPossedEntity;
    private Animator _animator;
    private Text _entityNameTextbox;
    private Text _dialogueTextbox;
    private Button _buttonPrefab;
    private Button _continueButton;
    private GameObject _questionPool;
    private Queue<string> _sentences = new Queue<string>();
    private Queue<Button> _choiceButtons = new Queue<Button>();
    private Question _dialogueContainedQuestion;
    private Sentence[] _defaultAnswers;
    private bool _isSentenceFinished = true;
    private bool _hasNoRelation;
    private float _typeSpeed;
    private int _maxDefaultSencteces = 0;

    private void Awake()
    {
        //Assign all variables in Dialogue Canvas
        _animator = GameObject.Find("DialogBox").GetComponent<Animator>();
        _entityNameTextbox = GameObject.Find("Name").GetComponent<Text>();
        _dialogueTextbox = GameObject.Find("Dialog").GetComponent<Text>();
        _buttonPrefab = Resources.Load<Button>("ScriptableObjects/Dialogue/Button");
        _continueButton = GameObject.Find("Continue").GetComponent<Button>();
        _questionPool = GameObject.Find("QuestionPool");
    }

    #region Conversation Trigger 
    public void TriggerConversation(bool isPossesing)
    {
        _hasNoRelation = false;
        _maxDefaultSencteces = 0;

        //Get textspeed from the playersettings
        PlayerSettings playerSettings = SaveHandler.Instance.LoadDataContainer<PlayerSettings>();
        if (playerSettings != null)
        {
            _typeSpeed = (float)playerSettings.TextSpeed / 10;
        }

        //ToDo: Remove this line later when interaction with the world is thought about by the lead dev and lead game designer.
        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, 5);
        foreach (Collider entityCollider in hitColliderArray)
        {
            //Check which entity boolia is possesing
            if (isPossesing)
            {
                _currentPossedEntity = PossessionBehaviour.PossessionTarget.GetComponent<IEntity>();
            }

            if (entityCollider.TryGetComponent(out IEntity entityToTalkTo))
            {
                //When possesing check if the 2 interacting NPC have a relationship
                //If realtionship count is 0 they have a realtionship with everyone
                if (isPossesing && entityToTalkTo != _currentPossedEntity && !entityToTalkTo.Relationships.Contains(_currentPossedEntity.CharacterName) && entityToTalkTo.Relationships.Count != 0)
                {
                    _hasNoRelation = true;
                    _defaultAnswers = entityToTalkTo.DefaultAnswers;
                }

                //start conversation if boolia is possesing another npc
                //or if she is not possesing anyone check if she can talk to the NPC
                if ((!isPossesing && entityToTalkTo.CanTalkToBoolia) ||
                    (isPossesing && entityToTalkTo != _currentPossedEntity))
                {
                    hasConversationStarted = true;
                    _entityNameTextbox.text = EnumValueToString(entityToTalkTo.CharacterName);
                    _animator.SetBool("IsOpen", true);
                    GameManager.CursorIsLocked = false;

                    ManageConversation(entityToTalkTo.Dialogue, entityToTalkTo.Question);
                    return;
                }
            }
        }
    }
    #endregion

    #region Manange conversation 
    public void ManageConversation(Dialogue dialogue, Question question)
    {
        ResetQuestions();
        //Check to ask question, start dialogue or end conversation
        if (dialogue && !_hasNoRelation)
        {
            StartDialogue(dialogue);
        }

        if (question && !_hasNoRelation)
        {
            AskQuestion(question);
        }

        if (_hasNoRelation)
        {
            StartDefaultDialogue(_defaultAnswers);
        }

        if (!question && !dialogue)
        {
            CloseConversation();
        }
    }

    public void CloseConversation()
    {
        hasConversationStarted = false;
        _animator.SetBool("IsOpen", false);
        GameManager.CursorIsLocked = true;
    }
    #endregion

    #region Dialogue
    private void StartDialogue(Dialogue dialogue)
    {
        _dialogueContainedQuestion = dialogue.question;
        _continueButton.gameObject.SetActive(true);
        _sentences.Clear();
        
        foreach (Sentence sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence.Text.ToString());
        }

        DisplayNextSentence();
    }

    private void StartDefaultDialogue(Sentence[] dialogue)
    {
        _continueButton.gameObject.SetActive(true);

        string sentence;

        if (dialogue.Length != 0)
        {
            int randomNumber = Random.Range(0, dialogue.Length);
            sentence = dialogue[randomNumber].Text.ToString();
        }
        else
        {
            sentence = "...";
        }

        StopAllCoroutines();
        _maxDefaultSencteces = 1;
        StartCoroutine(TypeSentence(sentence, _typeSpeed));
    }

    public void DisplayNextSentence()
    {
        if (!_isSentenceFinished)
        {
            return;
        }

        if (_sentences.Count == 0 || _maxDefaultSencteces == 1)
        {
            if (_dialogueContainedQuestion)
            {
                ManageConversation(null, _dialogueContainedQuestion);
                return;
            }

            ManageConversation(null, null);
            return;
        }

        string sentence = _sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence, _typeSpeed));
    }

    IEnumerator TypeSentence(string sentence, float typespeed)
    {
        _dialogueTextbox.text = "";
        _isSentenceFinished = false;

        foreach (char letter in sentence.ToCharArray())
        {
            _dialogueTextbox.text += letter;
            yield return new WaitForSeconds(typespeed);
        }

        _isSentenceFinished = true;
    }
    #endregion

    #region Question
    private void AskQuestion(Question question)
    {
        StartCoroutine(TypeSentence(question.Text.ToString(), _typeSpeed));
        _continueButton.gameObject.SetActive(false);

        foreach (Choice choice in question.Choices)
        {
            Button choiceButton = Instantiate(_buttonPrefab, Vector3.zero, Quaternion.identity);
            choiceButton.transform.SetParent(_questionPool.transform, false);
            choiceButton.GetComponentInChildren<Text>().text = choice.Text.ToString();
            choiceButton.onClick.AddListener(delegate () { ManageConversation(choice.Dialogue, choice.Question); });

            //If boolia is possesing the wrong NPC disable certain choiceButtons
            //If CharacterUnlocksChoice is 0 enable all choiceButtons
            if (_currentPossedEntity != null && !choice.CharacterUnlocksChoice.Contains(_currentPossedEntity.CharacterName) && choice.CharacterUnlocksChoice.Count != 0)
            {
                choiceButton.interactable = false;
            }

            _choiceButtons.Enqueue(choiceButton);
        }
    }

    private void ResetQuestions()
    {
        //Reset all buttons to orignial state if not used or there is a next question
        foreach(Button button in _choiceButtons)
        {
            if (button)
            {
                Destroy(button.gameObject);
            }
        }
    }
    #endregion
    private string EnumValueToString(CharacterList character)
    {
        string newValue = Regex.Replace(character.ToString(), "([a-z])([A-Z])", "$1 $2");

        return newValue;
    }
}