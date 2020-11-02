using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LoadingScreen : MonoBehaviour
{
    public Slider LoadingBar;
    public Text HintText;
    public string[] HintTextArray;

    private float _minimumLoadingTime = 2f;

    private void Awake()
    {
        ShowRandomHintInLoadScreen();
    }

    private void Start()
    {
        StartCoroutine(LoadLevelAsynchronously("LevitateScene"));
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

    private string GetRandomHintFromArray()
    {
        int randomIndex = Random.Range (0, HintTextArray.Length);
        return HintTextArray[randomIndex];
    }
}
