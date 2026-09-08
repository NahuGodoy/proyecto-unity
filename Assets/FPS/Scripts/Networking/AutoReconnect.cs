using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Unity.FPS.Networking
{
    public class AutoReconnect : MonoBehaviourPunCallbacks
    {
        public static void StartReconnect(string sceneName)
        {
            var go = new GameObject("AutoReconnect");
            DontDestroyOnLoad(go);
            var ar = go.AddComponent<AutoReconnect>();
            ar.TargetScene = sceneName;
        }

        public string TargetScene;
        bool requestedReconnect = false;
        bool waitingDisconnect = false;

        void Start()
        {
            if (!requestedReconnect)
            {
                requestedReconnect = true;
                Begin();
            }
        }

        void Begin()
        {
            Debug.Log("AutoReconnect: Begin reconnect flow");
            if (PhotonNetwork.IsConnected)
            {
                waitingDisconnect = true;
                PhotonNetwork.Disconnect();
            }
            else
            {
                Connect();
            }
        }

        void Connect()
        {
            Debug.Log("AutoReconnect: Connecting to Photon");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("AutoReconnect: Disconnected (" + cause + ")");
            // after disconnect, start connection
            waitingDisconnect = false;
            Connect();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("AutoReconnect: Connected to Master, joining/creating room");
            // Try to join random or create with CleanupCacheOnLeave=false so objects persist when players leave
            var roomOptions = new Photon.Realtime.RoomOptions { CleanupCacheOnLeave = false };
            PhotonNetwork.JoinRandomOrCreateRoom(null, 0, Photon.Realtime.MatchmakingMode.FillRoom, null, null, null, roomOptions);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"AutoReconnect: JoinRandom failed ({returnCode}): {message} - creating room");
            var opt = new RaiseEventOptions();
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 8 });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("AutoReconnect: Joined room, loading scene " + TargetScene);
            // Load the scene via Photon so all in-room clients sync
            PhotonNetwork.LoadLevel(TargetScene);
            // cleanup this manager after a small delay to allow scene load to begin
            Destroy(gameObject, 1f);
        }

        void Update()
        {
            // safety timeout: if stuck connecting for too long, show log
        }
    }
}
