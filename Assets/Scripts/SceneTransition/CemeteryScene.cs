using UnityEngine;

namespace SceneTransition
{
    public class CemeteryScene : MonoBehaviour
    {
        public string SceneName;

        public void LeaveCemeteryScene()
        {
            SceneTransitionHandler.Instance.GoToScene(SceneName ?? "MainMenu");
        }
    }
}