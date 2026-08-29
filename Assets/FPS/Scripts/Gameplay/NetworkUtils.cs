using UnityEngine;
using Photon.Pun;

namespace Unity.FPS.Gameplay
{
    public static class NetworkUtils
    {
        public static PlayerCharacterController GetLocalPlayer()
        {
            PlayerCharacterController[] players = Object.FindObjectsByType<PlayerCharacterController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

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
            if (target == null) return;

            // Modo Offline
            if (!PhotonNetwork.IsConnected)
            {
                Object.Destroy(target);
                return;
            }

            // Si el objeto tiene un PhotonView propio
            PhotonView targetPV = target.GetComponent<PhotonView>();
            if (targetPV != null)
            {
                if (targetPV.IsMine || PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(target);
                }
                return;
            }

            // Si es un pickup de la escena sin PhotonView:
            PlayerCharacterController localPlayer = GetLocalPlayer();
            if (localPlayer != null)
            {
                PhotonView playerPV = localPlayer.GetComponent<PhotonView>();
                if (playerPV != null)
                {
                    // Notificamos a todos que oculten este pickup especificando nombre y posición
                    playerPV.RPC("RPC_DisablePickup", RpcTarget.AllBuffered, target.name, target.transform.position);
                }
            }
        }
    }
}