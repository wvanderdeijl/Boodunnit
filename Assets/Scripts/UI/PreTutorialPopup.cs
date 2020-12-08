using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreTutorialPopup : MonoBehaviour
{
    [TextArea(3, 10)]
    public string[] PopupTexts;
    public GameObject NextButton;
    public GameObject CloseButton;
    public Text PopupText;

    private int _textCounter = 0;

    public void Awake()
    {
        if (PopupTexts != null && PopupTexts.Length >= 1)
        {
            PopupText.text = PopupTexts[0];
        }

        if (PopupTexts.Length > 1)
        {
            NextButton.SetActive(true);
            CloseButton.SetActive(false);
        }
    }

    public void OnNextClicked()
    {
        _textCounter++;
        if (PopupTexts.Length == _textCounter + 1)
        {
            PopupText.text = PopupTexts[_textCounter];
            NextButton.SetActive(false);
            CloseButton.SetActive(true);
        } 
        
        else
        {
            PopupText.text = PopupTexts[_textCounter];
        }
    }

    public void OnClosePressed()
    {
        GameManager.IsPaused = !GameManager.IsPaused;
        GameManager.CursorIsLocked = !GameManager.CursorIsLocked;
        Time.timeScale = GameManager.IsPaused ? 0 : 1;
    }

}
