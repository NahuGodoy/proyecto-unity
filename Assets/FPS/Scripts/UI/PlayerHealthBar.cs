using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        public Image HealthFillImage;
        private Health m_PlayerHealth;

        void Start()
        {
            TryBindPlayer();
        }

        void Update()
        {
            if (m_PlayerHealth == null)
            {
                TryBindPlayer();
                return;
            }

            if (HealthFillImage != null && m_PlayerHealth.MaxHealth > 0)
            {
                HealthFillImage.fillAmount = m_PlayerHealth.CurrentHealth / m_PlayerHealth.MaxHealth;
            }
        }

        private void TryBindPlayer()
        {
            // Una sola línea para obtener al jugador local
            PlayerCharacterController localPlayer = NetworkUtils.GetLocalPlayer();

            if (localPlayer != null)
            {
                m_PlayerHealth = localPlayer.GetComponent<Health>();
            }
        }
    }
}