using Enums;
using Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    public static bool hasConversationStarted;

    private Animator Animator;
    private Text EntityNameTextbox;
    private Text DialogueTextbox;
    private Button ContinueButton;
    private GameObject QuestionPool;
    public Button ButtonPrefab;

    private IEntity _currentPossedEntity;
    private Queue<string> _sentences = new Queue<string>();
    private Queue<Button> _choiceButtons = new Queue<Button>();
    private Question _dialogueContainedQuestion;
    private Sentence[] _defaultAnswers;
    private bool _isSentenceFinished = true;
    private bool _hasNoRelation;
    private float _typeSpeed;
    private int _keepCount = 0;

    private void Awake()
    {
        //To NOT change the gameobject names of the Dialog[Canvas] gameobject, that will break the below code
        GameObject conversationCanvasGO = GameObject.Find("Dialogue[Canvas]");
        Animator = conversationCanvasGO.GetComponentInChildren<Animator>();
        EntityNameTextbox = conversationCanvasGO.transform.Find("DialogBox").Find("NameBK").Find("Name").GetComponent<Text>();
        DialogueTextbox = conversationCanvasGO.transform.Find("DialogBox").Find("Dialog").GetComponent<Text>();
        ContinueButton = conversationCanvasGO.transform.Find("DialogBox").Find("Continue").GetComponent<Button>();
        QuestionPool = conversationCanvasGO.transform.Find("QuestionPool").gameObject;
    }

    public void TriggerConversation(bool isPossesing)
    {
        _hasNoRelation = false;
        _keepCount = 0;

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
            //Get the entity boolia is currently possesing
            if (isPossesing)
            {
                _currentPossedEntity = PossessionBehaviour.PossessionTarget.GetComponent<IEntity>();
            }

            if (entityCollider.TryGetComponent(out IEntity entityToTalkTo))
            {
                if (isPossesing && entityToTalkTo != _currentPossedEntity && !entityToTalkTo.Relationships.Contains(_currentPossedEntity.CharacterName) && entityToTalkTo.Relationships.Count != 0)
                {
                    _hasNoRelation = true;
                    _defaultAnswers = entityToTalkTo.DefaultAnswers;
                }

                //Start conversation when boolia is taling to emmie || ispossing and has relationship with NPC
                if ((!isPossesing && entityToTalkTo.CharacterName == CharacterList.EmmieLawson) || (isPossesing && entityToTalkTo != _currentPossedEntity))
                {
                    hasConversationStarted = true;
                    EntityNameTextbox.text = EnumValueToString(entityToTalkTo.CharacterName);
                    Animator.SetBool("IsOpen", true);
                    GameManager.CursorIsLocked = false;

                    ManageConversation(entityToTalkTo.Dialogue, entityToTalkTo.Question);
                    return;
                }
            }
        }
    }

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

        if (!question && !dialogue || _keepCount == 1)
        {
            CloseConversation();
        }
    }

    public void CloseConversation()
    {
        Animator.SetBool("IsOpen", false);
        hasConversationStarted = false;
        GameManager.CursorIsLocked = true;
    }

    /* Conversation */
    private void StartDialogue(Dialogue dialogue)
    {
        _dialogueContainedQuestion = dialogue.question;
        ContinueButton.gameObject.SetActive(true);
        _sentences.Clear();
        
        foreach (Sentence sentence in dialogue.sentences)
        {
            _sentences.Enqueue(sentence.Text.ToString());
        }

        DisplayNextSentence();
    }

    private void StartDefaultDialogue(Sentence[] dialogue)
    {
        ContinueButton.gameObject.SetActive(true);

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
        StartCoroutine(TypeSentence(sentence, _typeSpeed));
    }

    public void DisplayNextSentence()
    {
        if (!_isSentenceFinished)
        {
            return;
        }

        if (_sentences.Count == 0)
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
        DialogueTextbox.text = "";
        _isSentenceFinished = false;

        foreach (char letter in sentence.ToCharArray())
        {
            DialogueTextbox.text += letter;
            yield return new WaitForSeconds(typespeed);
        }

        _isSentenceFinished = true;
    }

    /* Question */
    private void AskQuestion(Question question)
    {
        StartCoroutine(TypeSentence(question.Text.ToString(), _typeSpeed));
        ContinueButton.gameObject.SetActive(false);

        foreach (Choice choice in question.Choices)
        {
            Button choiceButton = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity);
            choiceButton.transform.SetParent(QuestionPool.transform, false);
            choiceButton.GetComponentInChildren<Text>().text = choice.Text.ToString();
            choiceButton.onClick.AddListener(delegate () { ManageConversation(choice.Dialogue, choice.Question); });

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
    private string EnumValueToString(CharacterList character)
    {
        string newValue = Regex.Replace(character.ToString(), "([a-z])([A-Z])", "$1 $2");

        return newValue;
    }
}
