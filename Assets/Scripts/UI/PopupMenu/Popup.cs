using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject PopupMenuUI;
    public static bool isPopUpOpen;

    //private void Update()
    //{
    //    if (Input.anyKeyDown)
    //    {
    //        ClosePopup();
    //    }
    //}

    public void OpenPopup()
    {
        if (!isPopUpOpen)
        {
            DisableOrEnableOtherCanvasses(false);
            PopupMenuUI.SetActive(true);
            isPopUpOpen = true;
            Time.timeScale = 0f;
        }
    }

    public void ClosePopup()
    {
        DisableOrEnableOtherCanvasses(true);
        PopupMenuUI.SetActive(false);
        isPopUpOpen = false;
        Time.timeScale = 1f;
    }

    private void DisableOrEnableOtherCanvasses(bool shouldDisable)
    {
        Canvas[] canvasses = FindObjectsOfType<Canvas>();
        foreach(Canvas canvas in canvasses)
        {
            if(canvas != PopupMenuUI)
            {
                canvas.enabled = shouldDisable;
            }
        }
    }
}