using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        GameManager.CursorIsLocked = false;
        if (!SaveHandler.Instance.DoesSaveGameExist())
        {
            GameObject continueButton = GameObject.Find("ContinueGameButton");
            if (continueButton)
                continueButton.SetActive(false);
        }
    }

    public void NewGame()
    {
        // Todo start a new game
        SaveHandler.Instance.StartNewGame();
        SceneTransitionHandler.Instance.GoToScene("PreTutorialScene");
        GameManager.CursorIsLocked = true;
    }

    public void ContinueGame()
    {
        if (!SaveHandler.Instance.DoesSaveGameExist())
            return;
        // Todo load the game from the last save point
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
