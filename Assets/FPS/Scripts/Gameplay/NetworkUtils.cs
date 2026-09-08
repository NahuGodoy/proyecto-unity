using UnityEngine;
using Photon.Pun;

namespace Unity.FPS.Gameplay
{
    public static class NetworkUtils
    {
        public static PlayerCharacterController GetLocalPlayer()
        {
            PlayerCharacterController[] players = Object.FindObjectsByType<PlayerCharacterController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            // Offline: return the first found player (single-player mode)
            if (!PhotonNetwork.IsConnected)
            {
                return players != null && players.Length > 0 ? players[0] : null;
            }

            // Online: find the player whose PhotonView belongs to this client
            foreach (var player in players)
            {
                PhotonView pv = player.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                    return player;
            }

            return null;
        }

        public static void PickupDestroy(GameObject target)
        {
            if (target == null) return;

            Pickup pickup = target.GetComponent<Pickup>();
            if (pickup == null) return;

            // Modo Offline
            if (!PhotonNetwork.IsConnected)
            {
                pickup.RespawnAfterDelay();
                return;
            }

            // Scene pickups are hidden and restored through the player's owned RPC.
            PlayerCharacterController localPlayer = GetLocalPlayer();
            if (localPlayer != null)
            {
                PhotonView playerPV = localPlayer.GetComponent<PhotonView>();
                if (playerPV != null)
                {
                    double respawnAt = PhotonNetwork.Time + pickup.RespawnDelay;
                    playerPV.RPC(nameof(PlayerCharacterController.RPC_DisablePickup), RpcTarget.AllBuffered,
                        target.name, target.transform.position, respawnAt);
                }
            }
        }
    }
}