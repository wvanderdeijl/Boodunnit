using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public List<Step> StepsInCutscene;
    public static bool IsCutSceneFinished;

    private void Awake()
    {
        StartCutscene();
    }

    public void StartCutscene()
    {
        if (!GameManager.IsCutscenePlaying)
        {
            GameManager.IsCutscenePlaying = true;
            IsCutSceneFinished = false;
            DisableOrEnablePlayer(false);
            DisableOrEnablePlayerCamera(false);
        }
    }

    public void StopCutscene()
    {
        GameManager.IsCutscenePlaying = false;

        DisableOrEnablePlayer(true);
        DisableOrEnablePlayerCamera(true);
    }

    private void Update()
    {
        if (GameManager.IsCutscenePlaying)
        {
            PerformStepsInCutscene();
        }
    }

    private void PerformStepsInCutscene()
    {
        foreach (Step currentStep in StepsInCutscene)
        {
            StartCoroutine(currentStep.PerformActionsInCutscene());
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
