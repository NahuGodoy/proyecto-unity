using Photon.Pun;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    public class NetworkPickup : MonoBehaviourPun
    {
        public void DestroyPickup()
        {
            // Si estamos en singleplayer/offline
            if (!PhotonNetwork.IsConnected)
            {
                Destroy(gameObject);
                return;
            }

            // Si somos el Master Client,
            // podemos destruir directamente el pickup.
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
                return;
            }

            // Si no somos el Master,
            // le pedimos al Master que lo destruya.
            photonView.RPC(
                nameof(RequestDestroyPickup),
                RpcTarget.MasterClient
            );
        }

        [PunRPC]
        private void RequestDestroyPickup()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            PhotonNetwork.Destroy(gameObject);
        }
    }
}