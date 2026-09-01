using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Unity.FPS.Gameplay
{
      public class Goal : MonoBehaviour
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
            }

            SceneManager.LoadScene("WinScene");
        }
    }
  
}
