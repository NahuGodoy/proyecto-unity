using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class StanceHUD : MonoBehaviour
    {
        [Tooltip("Image component for the stance sprites")]
        public Image StanceImage;

        [Tooltip("Sprite to display when standing")]
        public Sprite StandingSprite;

        [Tooltip("Sprite to display when crouching")]
        public Sprite CrouchingSprite;

        PlayerCharacterController m_Character;

        void Start()
        {
            StartCoroutine(WaitForCharacterAndInit());
        }

        System.Collections.IEnumerator WaitForCharacterAndInit()
        {
            float timeout = 5f;
            float start = Time.time;
            PlayerCharacterController character = null;
            while (Time.time - start < timeout)
            {
                character = NetworkUtils.GetLocalPlayer();
                if (character != null)
                    break;
                yield return null;
            }

            if (character == null)
            {
                DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, StanceHUD>(character, this);
                yield break;
            }

            InitWithCharacter(character);
        }

        void InitWithCharacter(PlayerCharacterController character)
        {
            if (m_Character != null)
                m_Character.OnStanceChanged -= OnStanceChanged;

            m_Character = character;
            m_Character.OnStanceChanged += OnStanceChanged;
            OnStanceChanged(m_Character.IsCrouching);
        }

        void OnDestroy()
        {
            if (m_Character != null)
                m_Character.OnStanceChanged -= OnStanceChanged;
        }

        void OnStanceChanged(bool crouched)
        {
            StanceImage.sprite = crouched ? CrouchingSprite : StandingSprite;
        }
    }
}