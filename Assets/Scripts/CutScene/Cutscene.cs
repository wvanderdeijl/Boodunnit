using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public List<Action> ActionsInCutscene;
    private int _transitionSpeedMultiplier = 10;

    private void Awake()
    {
    }

    public void StartCutscene()
    {
        GameManager.IsCutscenePlaying = true;
        DisableOrEnablePlayer(false);
        DisableOrEnablePlayerCamera(false);

        StartCoroutine(ExecuteActions());
    }

    private IEnumerator ExecuteActions()
    {
        int actionCounter = 0;
        if (ActionsInCutscene == null || ActionsInCutscene.Count == 0)
        {
            yield break;
        }

        while (GameManager.IsCutscenePlaying)
        {
            Action action = ActionsInCutscene[actionCounter];
            action.IsExecuting = true;

            switch (action.ActionType)
            {
                case ActionType.Position:
                    yield return StartCoroutine(ChangePositionOfGameObject(action));
                    break;
                case ActionType.Rotation:
                    yield return StartCoroutine(ChangeRotationOfGameObject(action));
                    break;
                case ActionType.Popup:
                    yield return StartCoroutine(OpenPopUp(action));
                    break;
                case ActionType.Dialogue:
                    yield return StartCoroutine(StartDialogue(action));
                    break;
            }
           
            if (!action.IsExecuting)
            {
                actionCounter++;
                if (actionCounter <= ActionsInCutscene.Count - 1)
                {
                    yield return new WaitForSeconds(action.TimeBeforeNextAction);
                }
                else
                {
                    GameManager.IsCutscenePlaying = false;
                    DisableOrEnablePlayer(true);
                    DisableOrEnablePlayerCamera(true);
                    yield break;
                }
            }
            yield return null;
        }
    }

    private IEnumerator ChangePositionOfGameObject(Action currentAction)
    {
        if (!currentAction.ObjectForCutscene)
            yield break;

        GameObject gameObject = currentAction.ObjectForCutscene;
        if (currentAction.IsInstant)
        {
            gameObject.transform.position = currentAction.EndPosition;
            currentAction.IsExecuting = false;
        }

        while (currentAction.IsExecuting)
        {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, currentAction.EndPosition, currentAction.TransitionSpeed * Time.deltaTime);

            if (gameObject.transform.position == currentAction.EndPosition)
                currentAction.IsExecuting = false;

            yield return null;
        }
    }

    private IEnumerator ChangeRotationOfGameObject(Action currentAction)
    {
        if (!currentAction.ObjectForCutscene)
            yield break;

        GameObject gameObject = currentAction.ObjectForCutscene;
        if (currentAction.IsInstant)
        {
            Quaternion rotateToEndRotation = Quaternion.Euler(currentAction.EndRotation.x, currentAction.EndRotation.y, currentAction.EndRotation.z);
            gameObject.transform.rotation = rotateToEndRotation;
            currentAction.IsExecuting = false;
        }

        while (currentAction.IsExecuting)
        {
            Quaternion rotateToEndRotation = Quaternion.Euler(currentAction.EndRotation.x, currentAction.EndRotation.y, currentAction.EndRotation.z);
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, rotateToEndRotation, (currentAction.TransitionSpeed * _transitionSpeedMultiplier) * Time.deltaTime);

            if (gameObject.transform.rotation == rotateToEndRotation)
            {
                currentAction.IsExecuting = false;
            }

            yield return null;
        }
    }

    private IEnumerator OpenPopUp(Action currentAction)
    {
        if (currentAction.Popup == null) 
            yield break;

        currentAction.Popup.OpenPopup();
        
        while (currentAction.IsExecuting)
        {
            currentAction.IsExecuting = Popup.isPopUpOpen;
            yield return null;
        }
    }

    private IEnumerator StartDialogue(Action currentAction)
    {
        if (!currentAction.ObjectForCutscene)
            yield break;

        PlayerBehaviour playerBehaviour = currentAction.ObjectForCutscene.GetComponent<PlayerBehaviour>();
        if (playerBehaviour.GetComponent<ConversationManager>() && playerBehaviour.GetComponent<PossessionBehaviour>())
        {
            ConversationManager conversationManager = playerBehaviour.GetComponent<ConversationManager>();
            bool isPossessing = playerBehaviour.GetComponent<PossessionBehaviour>().IsPossessing;
            conversationManager.TriggerConversation(isPossessing);
        }

        while (currentAction.IsExecuting)
        {
            currentAction.IsExecuting = ConversationManager.hasConversationStarted;
            yield return null;
        }
    }

    
    /// <summary>
    /// Enable or disable everything bound the player movement and ability.
    /// </summary>
    /// <param name="shouldPlayerBeEnabled">Should player be enabled or disabled.</param>
    private void DisableOrEnablePlayer(bool shouldPlayerBeEnabled)
    {
        GameObject player = FindObjectInScene("Player");
        player.GetComponent<PlayerBehaviour>().enabled = shouldPlayerBeEnabled;
    }

    /// <summary>
    /// Enable or disable everything bound the the player camera.
    /// </summary>
    /// <param name="shouldCamereaBeEnabled"></param>
    private void DisableOrEnablePlayerCamera(bool shouldCamereaBeEnabled)
    {
        GameObject camera = Camera.main.gameObject;
        camera.GetComponent<CameraController>().enabled = shouldCamereaBeEnabled;
    }

    /// <summary>
    /// Find Gameobject by name.
    /// </summary>
    /// <param name="nameOfObject">Name of object you want to find.</param>
    /// <returns></returns>
    private GameObject FindObjectInScene(string nameOfObject)
    {
        GameObject objectToFind = GameObject.Find(nameOfObject);
        if (objectToFind)
        {
            return objectToFind;
        }
        return null;
    }
}