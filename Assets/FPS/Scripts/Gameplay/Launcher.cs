using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Unity.FPS.Gameplay{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public static Launcher Instance { get; private set; }
        public PhotonView playerPrefab;
        public Transform spawnPoint;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            Instance = this;
            // Ensure diagnostics exists for debugging network issues
            if (this.GetComponent<PhotonDiagnostics>() == null)
            {
                this.gameObject.AddComponent<PhotonDiagnostics>();
            }
            Debug.Log($"Launcher: Awake - IsConnected={PhotonNetwork.IsConnected} InRoom={PhotonNetwork.InRoom} IsConnectedAndReady={PhotonNetwork.IsConnectedAndReady}");

            // If we're already in a room (for example reloading the scene), instantiate the player immediately.
            if (PhotonNetwork.InRoom)
            {
                InstantiatePlayer();
                return;
            }

            // If connected to Photon but not in a room, try to join or create one.
            if (PhotonNetwork.IsConnected)
            {
                var roomOptions = new Photon.Realtime.RoomOptions { CleanupCacheOnLeave = false };
                PhotonNetwork.JoinRandomOrCreateRoom(null, 0, Photon.Realtime.MatchmakingMode.FillRoom, null, null, null, roomOptions);
                return;
            }

            // Otherwise, connect to Photon (MenuLauncher usually handles this when starting from menu).
            Debug.Log("Launcher: Not connected, calling ConnectUsingSettings()");
            PhotonNetwork.ConnectUsingSettings();
        }

        // Update is called once per frame
    public override void OnJoinedRoom()
    {
        Debug.Log($"Launcher: OnJoinedRoom - Room={(PhotonNetwork.CurrentRoom!=null?PhotonNetwork.CurrentRoom.Name:"<null>")} PlayerCount={(PhotonNetwork.CurrentRoom!=null?PhotonNetwork.CurrentRoom.PlayerCount:0)}");
        InstantiatePlayer();
    }

    public void InstantiatePlayer()
    {
        // Attempt to auto-resolve missing references to be more robust during development
        if (playerPrefab == null)
        {
            // Try to find a suitable player prefab in Resources whose name contains "player"
            var resources = Resources.LoadAll<GameObject>("");
            foreach (var go in resources)
            {
                if (go == null) continue;
                var pv = go.GetComponent<PhotonView>();
                if (pv != null && go.name.ToLower().Contains("player"))
                {
                    playerPrefab = pv;
                    Debug.LogWarning("Launcher: Auto-assigned playerPrefab from Resources: " + go.name);
                    break;
                }
            }
        }

        // If still missing, log and bail
        if (playerPrefab == null)
        {
            Debug.LogError("Launcher: playerPrefab is null. Place a player prefab (with PhotonView) in Resources or assign it in the Inspector.");
            return;
        }

        // If no spawn point assigned, pick a sensible default (first GameObject with "spawn" in name) or use world origin
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (spawnPoint != null)
        {
            spawnPos = spawnPoint.position;
            spawnRot = spawnPoint.rotation;
        }
        else
        {
            // try to find a named spawn object in scene
            var allTransforms = FindObjectsOfType<Transform>();
            foreach (var t in allTransforms)
            {
                if (t == null) continue;
                var nameLower = t.name.ToLower();
                if (nameLower.Contains("spawn"))
                {
                    spawnPos = t.position;
                    spawnRot = t.rotation;
                    Debug.LogWarning("Launcher: Auto-assigned spawnPoint from scene object: " + t.name);
                    break;
                }
            }
        }

        // Prevent double instantiation
        // If a local player already exists, don't instantiate again
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.IsMine && pv.gameObject != null && pv.gameObject.GetComponent<PlayerCharacterController>() != null)
                return;
        }

        Debug.Log("Launcher: Instantiating player prefab '" + playerPrefab.name + "' at " + spawnPos);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);

        // Request state from other players so we appear synchronized quickly
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            byte eventCode = 101; // REQUEST_SYNC handled by NetworkPresence (must be <200)
            object content = PhotonNetwork.LocalPlayer.ActorNumber;
            var options = new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others };
            var sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(eventCode, content, options, sendOptions);
            Debug.Log($"Launcher: Sent REQUEST_SYNC to others. PlayerListOthers count: {PhotonNetwork.PlayerListOthers.Length}");
            foreach (var p in PhotonNetwork.PlayerListOthers)
            {
                Debug.Log($"  Other player: ActorNumber={p.ActorNumber} UserId={p.UserId}");
            }
        }
    }

    public static void RespawnLocalPlayer()
    {
        if (Instance != null)
        {
            // Destroy any existing local player network object before instantiating a new one
            var views = UnityEngine.Object.FindObjectsOfType<PhotonView>();
            foreach (var pv in views)
            {
                if (pv.IsMine && pv.gameObject != null && pv.gameObject.GetComponent<PlayerCharacterController>() != null)
                {
                    try
                    {
                        PhotonNetwork.Destroy(pv.gameObject);
                    }
                    catch { }
                    break;
                }
            }

            Instance.InstantiatePlayer();
        }
    }
    }
}