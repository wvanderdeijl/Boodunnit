using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused;

    public GameObject Canvas;

    public List<GameObject> CanvasPanels;

    public void OnPauseGame()
    {
        Canvas.SetActive(true);
        IsPaused = true;

        Time.timeScale = 0;
    }

    public void OnUnpauseGame()
    {
        Canvas.SetActive(false);
        IsPaused = false;
        ResetPanels();

        Time.timeScale = 1;
    }

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ResetPanels()
    {
        foreach(GameObject panel in CanvasPanels)
        {
            panel.SetActive(false);
        }
        CanvasPanels[0].SetActive(true);
    }
}
