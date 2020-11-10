using UnityEngine;

public class PopupMenu : MonoBehaviour
{
    public GameObject PopupMenuUI;

    void Start()
    {
        OpenPopup();
    }

    private void OpenPopup()
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