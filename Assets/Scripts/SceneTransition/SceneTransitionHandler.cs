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
        if (!sceneNameToLoad.Trim().Equals(""))
        {
            SaveHandler.Instance.SaveCurrentScene(sceneNameToLoad);
            SceneManager.LoadScene("LoadingScene"); 
        }
    }
}
