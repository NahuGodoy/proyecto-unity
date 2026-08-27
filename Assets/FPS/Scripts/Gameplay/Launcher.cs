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
            PhotonNetwork.JoinRandomOrCreateRoom();
        }

        // Update is called once per frame
    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
    }
}