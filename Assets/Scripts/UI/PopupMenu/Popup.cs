using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public GameObject PopupMenuUI;

    [HideInInspector]
    public GameObject CloseButton;

    [HideInInspector]
    public GameObject PreviousBtn, NextBtn;
    public Sprite[] Sprites;
    public Image DisplayImage;
    public static bool isPopUpOpen;

    private int imageIndex = 0;
    private SceneTransitionHandler _sceneTransitionHandler;

    private void Awake()
    {
        _sceneTransitionHandler = new SceneTransitionHandler();
        CloseButton = transform.Find("CloseButton").gameObject;
        NextBtn = transform.Find("NextButton").gameObject;
        PreviousBtn = transform.Find("PreviousButton").gameObject;

        if (Sprites != null && Sprites.Length == 1)
        {
            NextBtn.SetActive(false);
            PreviousBtn.SetActive(false);
        }
    }

    public void OpenPopup()
    {
        if (!isPopUpOpen)
        {
            GameManager.CursorIsLocked = false;
            DisableOrEnableOtherCanvasses(false);
            PopupMenuUI.SetActive(true);
            imageIndex = 0;
            UpdateImage();
            isPopUpOpen = true;
            Time.timeScale = 0f;
        }
    }

    public void ClosePopup()
    {
        GameManager.CursorIsLocked = true;
        DisableOrEnableOtherCanvasses(true);
        PopupMenuUI.SetActive(false);
        isPopUpOpen = false;
        Time.timeScale = 1f;
    }

    public void NavigateToMainMenu()
    {
        if (!isPopUpOpen)
        {
            GameManager.CursorIsLocked = false;
            DisableOrEnableOtherCanvasses(true);
            PopupMenuUI.SetActive(false);
            isPopUpOpen = false;
            Time.timeScale = 1f;
            _sceneTransitionHandler.GoToScene("MainMenu");
        }
    }

    public void ShowNextImage()
    {
        if (imageIndex < Sprites.Length)
        {
            imageIndex++;
        }

        UpdateImage();
    }

    public void ShowPreviousImage()
    {
        if (imageIndex > 0)
        {
            imageIndex--;
        }

        UpdateImage();
    }

    private void UpdateImage()
    {
        DisplayImage.sprite = Sprites[imageIndex];

        if ((imageIndex + 1) == Sprites.Length)
        {
            CloseButton.SetActive(true);
            NextBtn.SetActive(false);
        }
        else if (imageIndex == 0)
        {
            PreviousBtn.SetActive(false);
        }
        else
        {
            PreviousBtn.SetActive(true);
            NextBtn.SetActive(true);
            CloseButton.SetActive(false);
        }
    }

    private void DisableOrEnableOtherCanvasses(bool shouldDisable)
    {
        Canvas[] canvasses = FindObjectsOfType<Canvas>();
        foreach(Canvas canvas in canvasses)
        {
            if(canvas.gameObject != PopupMenuUI)
            {
                canvas.enabled = shouldDisable;
            }
        }
    }
}