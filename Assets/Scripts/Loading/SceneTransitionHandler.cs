using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionHandler : MonoBehaviour
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

    public void GoToScene(string sceneName)
    {
        // niet naar loading screen, maar volgende screen. Loading screen wordt 
        // automatisch ingeladen.
        // todo
        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        GoToScene("LoadingScene");
    }
}
