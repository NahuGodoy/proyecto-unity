using System.Text;
using Photon.Pun;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace Unity.FPS.Gameplay
{
    [ExecuteAlways]
    public class PhotonDiagnostics : MonoBehaviour
    {
        public bool Visible = true;
        public KeyCode ToggleKey = KeyCode.F1;

        void Update()
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            if (Keyboard.current != null && Keyboard.current[Key.F1].wasPressedThisFrame)
            {
                Visible = !Visible;
            }
#else
            if (Input.GetKeyDown(ToggleKey))
            {
                Visible = !Visible;
            }
#endif
        }

        void OnGUI()
        {
            if (!Visible) return;

            GUILayout.BeginVertical("box", GUILayout.Width(480));
            GUILayout.Label($"Photon Connected: {PhotonNetwork.IsConnected}");
            GUILayout.Label($"Photon ConnectedAndReady: {PhotonNetwork.IsConnectedAndReady}");
            GUILayout.Label($"In Room: {PhotonNetwork.InRoom}");
            GUILayout.Label($"Ping: {PhotonNetwork.GetPing()} ms");

            var views = FindObjectsOfType<PhotonView>();
            GUILayout.Label($"PhotonViews found: {views.Length}");

            foreach (var v in views)
            {
                string owners = v.Owner == null ? "(none)" : v.Owner.ToString();
                int ownerNr = v.OwnerActorNr;
                int viewId = v.ViewID;
                bool isMine = v.IsMine;
                Vector3 pos = v.transform.position;
                int observed = v.ObservedComponents == null ? 0 : v.ObservedComponents.Count;

                GUILayout.BeginHorizontal();
                GUILayout.Label($"ID:{viewId} Owner:{ownerNr} IsMine:{isMine} Pos:{pos} Observed:{observed}", GUILayout.Width(440));
                if (GUILayout.Button("Dump", GUILayout.Width(40)))
                {
                    DumpPhotonView(v);
                }
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Dump All to Console"))
            {
                DumpAll();
            }

            GUILayout.EndVertical();
        }

        void DumpAll()
        {
            var views = FindObjectsOfType<PhotonView>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--- PhotonDiagnostics Dump All ---");
            sb.AppendLine($"Connected: {PhotonNetwork.IsConnected} ConnectedAndReady: {PhotonNetwork.IsConnectedAndReady} InRoom: {PhotonNetwork.InRoom} Ping: {PhotonNetwork.GetPing()}");
            foreach (var v in views)
            {
                sb.AppendLine(DumpInfoForView(v));
            }
            Debug.Log(sb.ToString());
        }

        void DumpPhotonView(PhotonView v)
        {
            Debug.Log(DumpInfoForView(v));
        }

        string DumpInfoForView(PhotonView v)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"PhotonView ID: {v.ViewID}");
            sb.AppendLine($"  IsMine: {v.IsMine}");
            sb.AppendLine($"  OwnerActorNr: {v.OwnerActorNr}");
            sb.AppendLine($"  CreatorActorNr: {v.CreatorActorNr}");
            sb.AppendLine($"  IsRoomView: {v.IsRoomView}");
            sb.AppendLine($"  Position: {v.transform.position}");
            if (v.InstantiationData != null && v.InstantiationData.Length > 0)
            {
                sb.AppendLine($"  InstantiationData: {string.Join(", ", v.InstantiationData)}");
            }
            else
            {
                sb.AppendLine("  InstantiationData: (none)");
            }

            if (v.ObservedComponents != null)
            {
                sb.AppendLine($"  ObservedComponents ({v.ObservedComponents.Count}):");
                for (int i = 0; i < v.ObservedComponents.Count; i++)
                {
                    var c = v.ObservedComponents[i];
                    sb.AppendLine($"    [{i}] {c.GetType().FullName}");
                }
            }
            else
            {
                sb.AppendLine("  ObservedComponents: (null)");
            }

            return sb.ToString();
        }
    }
}
