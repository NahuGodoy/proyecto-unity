using Unity.FPS.Gameplay;
using UnityEngine;
using Photon.Pun;

namespace Unity.FPS.Gameplay
{
    public class JetpackPickup : Pickup
    {
        protected override void OnPicked(PlayerCharacterController byPlayer)
        {
            PhotonView playerPV = byPlayer.GetComponent<PhotonView>();

            if (playerPV != null && PhotonNetwork.IsConnected && !playerPV.IsMine)
                return;

            var jetpack = byPlayer.GetComponent<Jetpack>();
            if (!jetpack)
                return;

            if (jetpack.TryUnlock())
            {
                PlayPickupFeedback();

                NetworkUtils.PickupDestroy(gameObject);
            }
        }
    }
}