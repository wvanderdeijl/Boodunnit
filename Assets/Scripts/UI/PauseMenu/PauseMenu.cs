using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject Canvas;
    public GameObject SettingsCanvas;

    public List<GameObject> CanvasPanels;

    public void TogglePauseGame()
    {
        if (GameManager.IsPaused)
        {
            ResetPanels();
        }
        GameManager.IsPaused = !GameManager.IsPaused;
        GameManager.CursorIsLocked = !GameManager.IsPaused;
        Canvas.SetActive(GameManager.IsPaused);
        Time.timeScale = GameManager.IsPaused ? 0 : 1;
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
