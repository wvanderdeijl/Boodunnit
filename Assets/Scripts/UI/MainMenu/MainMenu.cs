using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void NewGame()
    {
        // Todo start a new game
        SceneTransitionHandler.Instance.GoToScene("DevSandbox");
    }

    public void ContinueGame()
    {
        // Todo load the game from the last save point
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
