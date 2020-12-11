using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreen : MonoBehaviour
{
    public static bool GoToMainMenu;

    public Slider LoadingBar;
    public Text HintText;
    public Image LoadingBackground;
    public string[] HintTextArray;
    
    private float _minimumLoadingTime = 2f;
    private Sprite _loadingBackgroundSprite;
    private string _sceneName;

    [SerializeField] private Sprite _backgroundSpriteMockup;

    private void Awake()
    {
        InitializeLoadingScene();
    }

    private void Start()
    {
        StartCoroutine(LoadLevelAsynchronously(_sceneName));
    }

    private void InitializeLoadingScene()
    {
        if (GoToMainMenu)
        {
            _sceneName = "MainMenu";
        }
        else
        {
            _sceneName = SaveHandler.Instance.LoadCurrentScene();
        }
        GameManager.CursorIsLocked = true;

        _loadingBackgroundSprite = _backgroundSpriteMockup;
        HintText.text = GetRandomHintFromArray();
        LoadingBackground.sprite = _loadingBackgroundSprite;
    }

    private IEnumerator LoadLevelAsynchronously(string sceneName)
    {
        AsyncOperation asyncLevelLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLevelLoad.allowSceneActivation = false;

        while (!asyncLevelLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLevelLoad.progress / 0.9f);
            LoadingBar.value = progress;
            
            if (asyncLevelLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(_minimumLoadingTime);
                asyncLevelLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
    
    private string GetRandomHintFromArray()
    {
        int randomIndex = Random.Range (0, HintTextArray.Length);
        return HintTextArray[randomIndex];
    }
}
