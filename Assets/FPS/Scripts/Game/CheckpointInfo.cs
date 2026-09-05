using UnityEngine;


namespace Unity.FPS.Game
{
    public class CheckpointInfo : MonoBehaviour
    {
        [Tooltip("Optional respawn point. If empty, the platform transform is used.")]
        public Transform RespawnPoint;

        public Vector3 Position => RespawnPoint != null ? RespawnPoint.position : transform.position;
        public Quaternion Rotation => RespawnPoint != null ? RespawnPoint.rotation : transform.rotation;

        public void GetRespawnPose(out Vector3 position, out Quaternion rotation)
        {
            position = Position;
            rotation = Rotation;
        }
    }
}