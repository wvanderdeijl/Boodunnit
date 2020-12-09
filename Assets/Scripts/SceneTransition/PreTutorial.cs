using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreTutorial : MonoBehaviour
{
    public string SceneName;

    public void LeavePretutorial()
    {
        if (SceneName != null)
        {
            SceneTransitionHandler.Instance.GoToScene(SceneName);
        } 

        else
        {
            SceneTransitionHandler.Instance.GoToScene("MainMenu");
        }
    }
}
