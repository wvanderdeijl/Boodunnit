using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject SettingsCanvas;
    public CameraController CameraController;

    public List<GameObject> CanvasPanels;

    public void TogglePauseGame()
    {
        if (GameManager.IsPaused)
        {
            ResetPanels();
        }

        GameManager.IsPaused = !GameManager.IsPaused;
        Canvas.SetActive(GameManager.IsPaused);
        Time.timeScale = GameManager.IsPaused ? 0 : 1;

        //Hint: dont forget to check if player settings is not null, this just threw an error because it was not checked
        PlayerSettings playerSettings = SaveHandler.Instance.LoadDataContainer<PlayerSettings>();
        if (playerSettings != null)
        {
            CameraController.RotationSpeed = playerSettings.CameraSensitivity;
        }
    }

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetPanels()
    {
        SettingsCanvas.SetActive(false);

        foreach (GameObject panel in CanvasPanels)
        {
            panel.SetActive(false);
        }

        CanvasPanels[0].SetActive(true);
    }
}
