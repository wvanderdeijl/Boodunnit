using System;
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

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject == GameObject.Find("Player") && EmmieTrigger.HasVisitedEmmie) LeaveCemeteryScene();
        }
    }
}