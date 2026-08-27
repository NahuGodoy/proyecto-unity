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
    }
}