using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreen : MonoBehaviour
{
    public Slider LoadingBar;
    public Text HintText;
    public Image LoadingBackground;
    public string[] HintTextArray;
    
    private float _minimumLoadingTime = 2f;
    private Sprite _loadingBackgroundSprite;
    private string _sceneName;

    [SerializeField] private Sprite _backgroundSpriteMockup; //Mockup

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
        // todo: Get background sprite from list of sprites for chosen scene
        _sceneName = SaveHandler.Instance.LoadCurrentScene();
        SetSceneToLoadAsynchronously(_sceneName); // mockup
        SetLoadingBackgroundSprite(_backgroundSpriteMockup); // mockup
        
        ShowRandomHintInLoadScreen();
        ShowBackgroundImage();
    }

    private IEnumerator LoadLevelAsynchronously(string sceneName)
    {
        AsyncOperation asyncLevelLoad = SceneManager.LoadSceneAsync(sceneName);

        asyncLevelLoad.allowSceneActivation = false;

        while (!asyncLevelLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLevelLoad.progress / 0.9f);
            UpdateLoadingBarValue(progress);
            
            if (asyncLevelLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(_minimumLoadingTime);
                asyncLevelLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private void UpdateLoadingBarValue(float progress)
    {
        LoadingBar.value = progress;
    }

    private void ShowRandomHintInLoadScreen()
    {
        HintText.text = GetRandomHintFromArray();
    }

    private void ShowBackgroundImage()
    {
        LoadingBackground.sprite = _loadingBackgroundSprite;
    }
    
    public void SetSceneToLoadAsynchronously(string sceneName)
    {
        _sceneName = sceneName;
    }

    public void SetLoadingBackgroundSprite(Sprite backgroundSprite)
    {
        _loadingBackgroundSprite = backgroundSprite;
    }
    
    private string GetRandomHintFromArray()
    {
        int randomIndex = Random.Range (0, HintTextArray.Length);
        return HintTextArray[randomIndex];
    }
}
