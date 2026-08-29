using UnityEngine;
using Photon.Pun;

namespace Unity.FPS.Gameplay
{
    public static class NetworkUtils
    {
        /// <summary>
        /// Busca y devuelve el PlayerCharacterController
        /// del jugador local.
        /// </summary>
        public static PlayerCharacterController GetLocalPlayer()
        {
            PlayerCharacterController[] players =
                Object.FindObjectsByType<PlayerCharacterController>(
                    FindObjectsInactive.Exclude
                );

            foreach (var player in players)
            {
                PhotonView pv = player.GetComponent<PhotonView>();

                if (pv == null || !PhotonNetwork.IsConnected || pv.IsMine)
                {
                    return player;
                }
            }

            return null;
        }

        public static void PickupDestroy(GameObject target)
        {
            if (target == null)
                return;

            NetworkPickup networkPickup =
                target.GetComponent<NetworkPickup>();

            if (networkPickup != null)
            {
                networkPickup.DestroyPickup();
                return;
            }

            // Fallback para objetos que no utilizan NetworkPickup
            PhotonView pickupPV = target.GetComponent<PhotonView>();

            if (pickupPV != null && PhotonNetwork.IsConnected)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(target);
                }
            }
            else
            {
                Object.Destroy(target);
            }
        }
    }
}