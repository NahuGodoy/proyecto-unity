using UnityEngine;
using Photon.Pun;

namespace Unity.FPS.Gameplay{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public PhotonView playerPrefab;
        public Transform spawnPoint;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            // If we're already in a room (for example reloading the scene), instantiate the player immediately.
            if (PhotonNetwork.InRoom)
            {
                InstantiatePlayer();
                return;
            }

            // If connected to Photon but not in a room, try to join or create one.
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomOrCreateRoom();
                return;
            }

            // Otherwise, connect to Photon (MenuLauncher usually handles this when starting from menu).
            PhotonNetwork.ConnectUsingSettings();
        }

        // Update is called once per frame
    public override void OnJoinedRoom()
    {
        InstantiatePlayer();
    }

    void InstantiatePlayer()
    {
        if (playerPrefab == null || spawnPoint == null)
            return;

        // Prevent double instantiation
        // If a local player already exists, don't instantiate again
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.IsMine && pv.gameObject != null && pv.gameObject.GetComponent<PlayerCharacterController>() != null)
                return;
        }

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
    }
}