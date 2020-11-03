using UnityEngine.SceneManagement;

public class SceneTransitionHandler
{
    public static SceneTransitionHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SceneTransitionHandler();
            }
            
            return _instance;
        }
    }
    
    private static SceneTransitionHandler _instance;

    public void GoToScene(string sceneNameToLoad)
    {
        // todo: daryl's SaveHandler saves next scene to load with param
        SceneManager.LoadScene("LoadingScene");
    }
}
