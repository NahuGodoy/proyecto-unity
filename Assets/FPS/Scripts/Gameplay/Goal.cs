using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Unity.FPS.Gameplay
{
    public class Goal : MonoBehaviourPun
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            PhotonView playerPhotonView = other.GetComponentInParent<PhotonView>();

            if (PhotonNetwork.IsConnected && playerPhotonView != null)
            {
                if (!playerPhotonView.IsMine)
                    return;

                photonView.RPC("GameOver", RpcTarget.All, playerPhotonView.Owner.ActorNumber);
            }
            else
            {
                SceneManager.LoadScene("WinScene");
            }
        }

        [PunRPC]
        void GameOver(int winnerID)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == winnerID)
                SceneManager.LoadScene("WinScene");
            else
                SceneManager.LoadScene("LoseScene");
        }
    }
}
