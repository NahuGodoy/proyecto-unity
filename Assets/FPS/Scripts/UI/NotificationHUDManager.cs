using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;

namespace Unity.FPS.UI
{
    public class NotificationHUDManager : MonoBehaviour
    {
        [Tooltip("UI panel containing the layoutGroup for displaying notifications")]
        public RectTransform NotificationPanel;

        [Tooltip("Prefab for the notifications")]
        public GameObject NotificationPrefab;

        PlayerWeaponsManager m_PlayerWeaponsManager;
        Jetpack m_Jetpack;

        void Awake()
        {
            EventManager.AddListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);
        }

        void Start()
        {
            TryBindLocalPlayer();
        }

        void Update()
        {
            // Si el jugador local aún no se ha vinculado (por tardar en instanciarse por red), reintentamos en Update
            if (m_PlayerWeaponsManager == null || m_Jetpack == null)
            {
                TryBindLocalPlayer();
            }
        }

        private void TryBindLocalPlayer()
        {
            PlayerCharacterController localPlayer = NetworkUtils.GetLocalPlayer();

            if (localPlayer != null)
            {
                // Vinculación de PlayerWeaponsManager
                if (m_PlayerWeaponsManager == null)
                {
                    m_PlayerWeaponsManager = localPlayer.GetComponent<PlayerWeaponsManager>();
                    if (m_PlayerWeaponsManager != null)
                    {
                        m_PlayerWeaponsManager.OnAddedWeapon += OnPickupWeapon;
                    }
                }

                // Vinculación de Jetpack
                if (m_Jetpack == null)
                {
                    m_Jetpack = localPlayer.GetComponent<Jetpack>();
                    if (m_Jetpack != null)
                    {
                        m_Jetpack.OnUnlockJetpack += OnUnlockJetpack;
                    }
                }
            }
        }

        void OnObjectiveUpdateEvent(ObjectiveUpdateEvent evt)
        {
            if (!string.IsNullOrEmpty(evt.NotificationText))
                CreateNotification(evt.NotificationText);
        }

        void OnPickupWeapon(WeaponController weaponController, int index)
        {
            if (index != 0)
                CreateNotification("Picked up weapon : " + weaponController.WeaponName);
        }

        void OnUnlockJetpack(bool unlock)
        {
            CreateNotification("Jetpack unlocked");
        }

        public void CreateNotification(string text)
        {
            GameObject notificationInstance = Instantiate(NotificationPrefab, NotificationPanel);
            notificationInstance.transform.SetSiblingIndex(0);

            NotificationToast toast = notificationInstance.GetComponent<NotificationToast>();
            if (toast)
            {
                toast.Initialize(text);
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<ObjectiveUpdateEvent>(OnObjectiveUpdateEvent);

            // Desvinculamos los eventos para prevenir memory leaks
            if (m_PlayerWeaponsManager != null)
            {
                m_PlayerWeaponsManager.OnAddedWeapon -= OnPickupWeapon;
            }

            if (m_Jetpack != null)
            {
                m_Jetpack.OnUnlockJetpack -= OnUnlockJetpack;
            }
        }
    }
}