#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Unity.FPS.Gameplay;

public static class PhotonPrefabValidator
{
    [MenuItem("Tools/Photon/Validate Player Prefabs")]
    public static void ValidatePlayerPrefabs()
    {
        int issues = 0;

        // Find all Launcher instances in the project (including prefabs and scenes)
        Launcher[] launchers = Resources.FindObjectsOfTypeAll<Launcher>();

        if (launchers == null || launchers.Length == 0)
        {
            Debug.LogWarning("Photon Prefab Validator: No Launcher instances found in project scenes or assets.");
            return;
        }

        foreach (var launcher in launchers)
        {
            if (launcher == null)
                continue;

            var pv = launcher.playerPrefab;
            if (pv == null)
            {
                Debug.LogError($"[Photon Validator] Launcher '{launcher.gameObject.name}' has no playerPrefab assigned.");
                issues++;
                continue;
            }

            string prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(pv.gameObject);
            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError($"[Photon Validator] Could not determine asset path for playerPrefab referenced by Launcher '{launcher.gameObject.name}'.");
                issues++;
                continue;
            }

            if (!prefabPath.Contains("/Resources/") && !prefabPath.Contains("Resources\\"))
            {
                Debug.LogWarning($"[Photon Validator] playerPrefab '{pv.gameObject.name}' is not inside a Resources folder. PhotonNetwork.Instantiate requires the prefab to be in Resources or a custom PrefabPool. Path: {prefabPath}");
                issues++;
            }

            // Check PhotonView presence on root
            var photonView = pv.gameObject.GetComponent<PhotonView>();
            if (photonView == null)
            {
                Debug.LogError($"[Photon Validator] playerPrefab '{pv.gameObject.name}' does not have a PhotonView component on root.");
                issues++;
            }

            var playerCC = pv.gameObject.GetComponent<PlayerCharacterController>();
            if (playerCC == null)
            {
                Debug.LogWarning($"[Photon Validator] playerPrefab '{pv.gameObject.name}' does not contain a PlayerCharacterController on root. Path: {prefabPath}");
                // Not always necessary, just warn
            }

            Debug.Log($"[Photon Validator] Validated playerPrefab '{pv.gameObject.name}' at {prefabPath}");
        }

        if (issues == 0)
        {
            EditorUtility.DisplayDialog("Photon Prefab Validator", "No critical issues found.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Photon Prefab Validator", $"Validation finished with {issues} issues (check console).", "OK");
        }
    }
}
#endif