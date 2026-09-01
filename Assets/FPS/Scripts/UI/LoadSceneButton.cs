using Unity.FPS.Game;
using UnityEngine;
using Photon.Pun;
using Unity.FPS.Gameplay;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using System.Linq;
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
            if (m_SubmitAction != null)
                m_SubmitAction.Enable();
            else
                Debug.LogWarning("LoadSceneButton: 'UI/Submit' InputAction not found.");
        }
        
        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject
                && m_SubmitAction != null
                && m_SubmitAction.WasPressedThisFrame())
            {
                LoadTargetScene();
            }
        }

        public void LoadTargetScene()
        {
            // If connected and the target scene is the gameplay scene, request respawn instead of reloading
                // For a full "start again" behavior, disconnect gracefully and reload the scene
                // so this client performs a fresh join (equivalent to pressing Play initially).
                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                {
                    if (SceneManager.GetActiveScene().name == SceneName)
                    {
                        Debug.Log("LoadSceneButton: triggering AutoReconnect for scene '" + SceneName + "'.");
                        // Start an automatic reconnect flow that will disconnect, reconnect and join/load
                        StartAutoReconnect(SceneName);
                        return;
                    }
                }

            // If we're connected and in a Photon room and want to switch scene, use PhotonNetwork.LoadLevel
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                PhotonNetwork.LoadLevel(SceneName);
                return;
            }

                // Otherwise, if explicitly configured to disconnect before load, do a full disconnect then load
                if (DisconnectBeforeLoad && PhotonNetwork.IsConnected)
                {
                    Debug.Log("LoadSceneButton: DisconnectBeforeLoad -> performing full disconnect before loading '" + SceneName + "'.");
                    StartCoroutine(DisconnectAndLoad(SceneName));
                    return;
                }

                SceneManager.LoadScene(SceneName);
        }

            void StartAutoReconnect(string sceneName)
            {
                // Try to find the AutoReconnect type in loaded assemblies
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Type arType = null;
                foreach (var a in assemblies)
                {
                    arType = a.GetType("Unity.FPS.Networking.AutoReconnect");
                    if (arType != null)
                        break;
                }

                if (arType == null)
                {
                    // fallback: try assembly-qualified name for Assembly-CSharp
                    arType = Type.GetType("Unity.FPS.Networking.AutoReconnect, Assembly-CSharp");
                }

                if (arType == null)
                {
                    Debug.LogError("StartAutoReconnect: AutoReconnect type not found in loaded assemblies.");
                    return;
                }

                var go = new GameObject("AutoReconnect");
                DontDestroyOnLoad(go);
                var comp = go.AddComponent(arType);
                // set TargetScene property if present
                var prop = arType.GetProperty("TargetScene");
                if (prop != null && prop.CanWrite)
                    prop.SetValue(comp, sceneName, null);
            }

            System.Collections.IEnumerator DisconnectAndLoad(string scene)
            {
                if (PhotonNetwork.IsConnected)
                {
                    PhotonNetwork.Disconnect();
                    // wait until disconnected
                    while (PhotonNetwork.IsConnected)
                        yield return null;
                }

                SceneManager.LoadScene(scene);
            }
    }
}