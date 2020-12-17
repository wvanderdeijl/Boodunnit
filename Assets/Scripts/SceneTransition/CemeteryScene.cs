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

        private void OnTriggerStay(Collider other)
        {
            if(other.gameObject == FindObjectOfType<PlayerBehaviour>().gameObject && !ConversationManager.HasConversationStarted) LeaveCemeteryScene();
        }
    }
}