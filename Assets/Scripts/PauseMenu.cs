using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused;

    public GameObject Canvas;

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

        Time.timeScale = 1;
    }

    public void OnQuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
