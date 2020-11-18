using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Step
{
    public List<Action> ActionsInThisStep;

    public IEnumerator PerformActionsInCutscene()
    {
        foreach (Action currentAction in ActionsInThisStep)
        {
            /**
                * 1. Check if a gameobject is set.
                *      1.1 Set begin position and rotation.
                *      1.2 Set end position and rotation.
                *      1.3 Keep in mind the time to transition.
                * 2. Check if action contains a Popup.
                *      2.1 Trigger Popup.
                * 3. Check if action contains a dialogue.
                *      3.1 Trigger Dialogue.
                * 4. Check if action is a blocking action. ???
            **/

            Debug.Log("Called");
            if (currentAction.ObjectForCutscene)
            {
                PerformActionWithGameObject(currentAction);
            }

            if (currentAction.Popup)
            {
                currentAction.Popup.OpenPopup();
            }
            // Check if dialogue is available.
        }
        
        // yield return new WaitUntil(() => Cutscene.IsCutSceneFinished);
    }

    private void PerformActionWithGameObject(Action currentAction)
    {
        GameObject objectInScene = currentAction.ObjectForCutscene;
        if (!currentAction.shouldUseCurrentObjectPosition)
        {
            objectInScene.transform.position = currentAction.StartPosition;
            objectInScene.transform.rotation = currentAction.StartRotation;
        }
        objectInScene.transform.position = Vector3.MoveTowards(objectInScene.transform.position, currentAction.EndPosition, currentAction.TransitionSpeed * Time.deltaTime);
    }
}
