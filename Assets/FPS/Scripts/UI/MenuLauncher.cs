using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Unity.FPS.UI
{
    public class MenuLauncher : MonoBehaviourPunCallbacks
    {
        public string SceneName = ""; 
        bool pendingConnect = false;
        string pendingScene = null;

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

            Debug.Log($"MenuLauncher: OnClickConnect - IsConnected={PhotonNetwork.IsConnected} IsConnectedAndReady={PhotonNetwork.IsConnectedAndReady} InRoom={PhotonNetwork.InRoom}");
            // Force a clean connect: if currently connected, disconnect first then connect.
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("MenuLauncher: currently connected - will disconnect and reconnect for a clean session.");
                pendingConnect = true;
                pendingScene = SceneName;
                PhotonNetwork.Disconnect();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log($"MenuLauncher: OnConnectedToMaster - loading scene {SceneName}. CurrentRoom={(PhotonNetwork.CurrentRoom!=null?PhotonNetwork.CurrentRoom.Name:"<null>")} PlayersInRoom={(PhotonNetwork.CurrentRoom!=null?PhotonNetwork.CurrentRoom.PlayerCount:0)}");
            // If we had a pendingScene from a forced reconnect, use that, otherwise use configured SceneName
            var toLoad = pendingScene ?? SceneName;
            pendingConnect = false;
            pendingScene = null;
            SceneManager.LoadScene(toLoad);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log($"MenuLauncher: OnDisconnected ({cause}). pendingConnect={pendingConnect}");
            if (pendingConnect)
            {
                Debug.Log("MenuLauncher: initiating ConnectUsingSettings after clean disconnect");
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }
}