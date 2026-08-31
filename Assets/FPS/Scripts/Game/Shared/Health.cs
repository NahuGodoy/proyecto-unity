using UnityEngine;
using UnityEngine.Events;
using Photon.Pun; // Requiere Photon

namespace Unity.FPS.Game
{
    public class Health : MonoBehaviour
    {
        [Tooltip("Maximum amount of health")] public float MaxHealth = 10f;

        [Tooltip("Health ratio at which the critical health vignette starts appearing")]
        public float CriticalHealthRatio = 0.3f;

        public UnityAction<float, GameObject> OnDamaged;
        public UnityAction<float> OnHealed;
        public UnityAction OnDie;

        public float CurrentHealth { get; set; }
        public bool Invincible { get; set; }
        public bool CanPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;
        public bool IsCritical() => GetRatio() <= CriticalHealthRatio;

        bool m_IsDead;
        private PhotonView m_PhotonView;

        void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();
        }

        void Start()
        {
            CurrentHealth = MaxHealth;
        }

        // --- HEAL ---
        public void Heal(float healAmount)
        {
            if (m_PhotonView != null && PhotonNetwork.IsConnected)
            {
                m_PhotonView.RPC(nameof(RPC_Heal), RpcTarget.All, healAmount);
            }
            else
            {
                RPC_Heal(healAmount);
            }
        }

        [PunRPC]
        private void RPC_Heal(float healAmount)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth += healAmount;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            float trueHealAmount = CurrentHealth - healthBefore;
            if (trueHealAmount > 0f)
            {
                OnHealed?.Invoke(trueHealAmount);
            }
        }

        // --- TAKE DAMAGE ---
        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible)
                return;

            if (m_PhotonView != null && PhotonNetwork.IsConnected)
            {
                // Enviar la orden de daño a todos los clientes por red
                m_PhotonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
            }
            else
            {
                RPC_TakeDamage(damage);
            }
        }

        [PunRPC]
        private void RPC_TakeDamage(float damage)
        {
            float healthBefore = CurrentHealth;
            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            float trueDamageAmount = healthBefore - CurrentHealth;
            if (trueDamageAmount > 0f)
            {
                OnDamaged?.Invoke(trueDamageAmount, null);
            }

            HandleDeath();
        }

        // --- KILL ---
        public void Kill()
        {
            if (m_PhotonView != null && PhotonNetwork.IsConnected)
            {
                m_PhotonView.RPC(nameof(RPC_Kill), RpcTarget.All);
            }
            else
            {
                RPC_Kill();
            }
        }

        [PunRPC]
        private void RPC_Kill()
        {
            CurrentHealth = 0f;
            OnDamaged?.Invoke(MaxHealth, null);
            HandleDeath();
        }

        void HandleDeath()
        {
            if (m_IsDead)
                return;

            if (CurrentHealth <= 0f)
            {
                m_IsDead = true;
                OnDie?.Invoke();
                // Ensure the player GameObject is removed for all clients.
                if (m_PhotonView != null && PhotonNetwork.IsConnected)
                {
                    // If we own this PhotonView, destroy it directly
                    if (m_PhotonView.IsMine)
                    {
                        PhotonNetwork.Destroy(m_PhotonView.gameObject);
                        // no ghost system anymore; rely on PhotonNetwork.Destroy to remove network object
                    }
                    else
                    {
                        // Ask the owner to destroy it (RPC to owner). If owner is missing, ask the MasterClient.
                        if (m_PhotonView.Owner != null)
                        {
                            m_PhotonView.RPC(nameof(RPC_RequestDestroy), m_PhotonView.Owner);
                        }
                        else
                        {
                            m_PhotonView.RPC(nameof(RPC_RequestDestroy), RpcTarget.MasterClient);
                        }
                    }
                }
            }
        }

        [PunRPC]
        void RPC_RequestDestroy()
        {
            if (m_PhotonView == null)
                return;

            if (m_PhotonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(m_PhotonView.gameObject);
            }
        }
    }
}