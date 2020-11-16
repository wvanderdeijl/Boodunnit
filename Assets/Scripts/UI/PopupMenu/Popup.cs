using UnityEngine;

public class Popup : MonoBehaviour
{
    public GameObject PopupMenuUI;

    void Awake()
    {

    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            ClosePopup();
        }
    }

    public void OpenPopup()
    {
        PopupMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePopup()
    {
        PopupMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }
}