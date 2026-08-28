using UnityEngine;
using Photon.Pun;


namespace Unity.FPS.Gameplay
{
    public static class NetworkUtils
    {
        /// <summary>
        /// Busca y devuelve el PlayerCharacterController del jugador local (o el de Singleplayer/Offline).
        /// </summary>
        public static PlayerCharacterController GetLocalPlayer()
        {
            PlayerCharacterController[] players = Object.FindObjectsByType<PlayerCharacterController>(FindObjectsInactive.Exclude);

            foreach (var player in players)
            {
                PhotonView pv = player.GetComponent<PhotonView>();

                // Si es offline / sin Photon O es nuestro jugador local en red
                if (pv == null || !PhotonNetwork.IsConnected || pv.IsMine)
                {
                    return player;
                }
            }

            return null;
        }
        public static void PickupDestroy(GameObject target)
        {
            if (target==null)
                return;

            PhotonView pickupPV = target.GetComponent<PhotonView>();
            if (pickupPV != null && PhotonNetwork.IsConnected)
            {
                if (pickupPV.IsMine || PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(target);
                }
                else
                {
                    target.SetActive(false);
                }
            }
            else
            {
                Object.Destroy(target);
            }
        }
    }
}