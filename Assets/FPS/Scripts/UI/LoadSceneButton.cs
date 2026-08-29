using Unity.FPS.Game;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Unity.FPS.UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        public string SceneName = "";
        public bool DisconnectBeforeLoad = false;

        private InputAction m_SubmitAction;
        
        void Start()
        {
            m_SubmitAction = InputSystem.actions.FindAction("UI/Submit");
            m_SubmitAction.Enable();
        }
        
        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject
                && m_SubmitAction.WasPressedThisFrame())
            {
                LoadTargetScene();
            }
        }

        public void LoadTargetScene()
        {
            if (DisconnectBeforeLoad && PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }

            SceneManager.LoadScene(SceneName);
        }
    }
}