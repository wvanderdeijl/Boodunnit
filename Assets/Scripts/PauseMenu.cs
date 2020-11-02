using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused;

    public GameObject Canvas;
    public GameObject SettingsCanvas;

    public List<GameObject> CanvasPanels;

    public void TogglePauseGame(bool isActiveCanvas, int timeScale)
    {
        Canvas.SetActive(isActiveCanvas);
        IsPaused = !IsPaused;

        Time.timeScale = timeScale;
    }

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ResetPanels()
    {
        SettingsCanvas.SetActive(false);
        foreach (GameObject panel in CanvasPanels)
        {
            panel.SetActive(false);
        }
        CanvasPanels[0].SetActive(true);
    }
}
