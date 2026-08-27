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
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}