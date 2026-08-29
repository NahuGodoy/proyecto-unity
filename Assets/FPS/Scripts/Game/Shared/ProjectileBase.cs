using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

namespace Unity.FPS.Game
{
    // Base class for projectiles that supports optional Photon network initialization
    public abstract class ProjectileBase : MonoBehaviourPun
    {
        public GameObject Owner { get; private set; }
        public Vector3 InitialPosition { get; private set; }
        public Vector3 InitialDirection { get; private set; }
        public Vector3 InheritedMuzzleVelocity { get; private set; }
        public float InitialCharge { get; private set; }

        public UnityAction OnShoot;

        bool m_InitializedFromNetwork = false;

        void Start()
        {
            // If instantiated over the network, Photon sets InstantiationData on the PhotonView.
            // Use that data to initialize this projectile on all clients.
            if (photonView != null && photonView.InstantiationData != null && photonView.InstantiationData.Length > 0)
            {
                ApplyInstantiationData(photonView.InstantiationData);
                m_InitializedFromNetwork = true;
            }
        }

        void ApplyInstantiationData(object[] data)
        {
            // expected data: ownerViewId (int), direction (Vector3), inheritedVelocity (Vector3), initialCharge (float), initialPosition (Vector3)
            try
            {
                int ownerViewId = (int)data[0];
                Vector3 direction = (Vector3)data[1];
                Vector3 inheritedVelocity = (Vector3)data[2];
                float initialCharge = (float)data[3];
                Vector3 initialPosition = (Vector3)data[4];

                if (ownerViewId != 0)
                {
                    PhotonView ownerPV = PhotonView.Find(ownerViewId);
                    if (ownerPV != null)
                        Owner = ownerPV.gameObject;
                }

                InitialDirection = direction;
                InheritedMuzzleVelocity = inheritedVelocity;
                InitialCharge = initialCharge;
                InitialPosition = initialPosition;

                transform.position = initialPosition;

                OnShoot?.Invoke();
            }
            catch
            {
                // ignore malformed instantiation data
            }
        }

        public void Shoot(WeaponController controller)
        {
            // Local (non-networked) initialization path
            Owner = controller.Owner;
            InitialPosition = transform.position;
            InitialDirection = transform.forward;
            InheritedMuzzleVelocity = controller.MuzzleWorldVelocity;
            InitialCharge = controller.CurrentCharge;

            // Only invoke OnShoot immediately for local instances
            if (!m_InitializedFromNetwork)
                OnShoot?.Invoke();
        }
    }
}