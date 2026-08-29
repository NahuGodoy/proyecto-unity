using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Unity.FPS.UI
{
    public class MenuLauncher : MonoBehaviourPunCallbacks
    {
        public string SceneName = ""; 

        public void OnClickConnect()
        {
            // If already connected and ready, just load the scene
            if (PhotonNetwork.IsConnectedAndReady)
            {
                SceneManager.LoadScene(SceneName);
                return;
            }

            // Increase send rate for smoother movement visuals during testing
            PhotonNetwork.SendRate = 20;

            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}