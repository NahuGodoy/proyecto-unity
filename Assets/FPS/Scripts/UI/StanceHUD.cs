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

        void Start()
        {
            PlayerCharacterController character = FindAnyObjectByType<PlayerCharacterController>();
            if (character == null)
            {
                DebugUtility.HandleErrorIfNullFindObject<PlayerCharacterController, StanceHUD>(character, this);
                StartCoroutine(WaitForCharacterAndInit());
                return;
            }

            InitWithCharacter(character);
        }

        System.Collections.IEnumerator WaitForCharacterAndInit()
        {
            float timeout = 5f;
            float start = Time.time;
            PlayerCharacterController character = null;
            while (Time.time - start < timeout)
            {
                character = FindAnyObjectByType<PlayerCharacterController>();
                if (character != null)
                    break;
                yield return null;
            }

            if (character == null)
                yield break;

            InitWithCharacter(character);
        }

        void InitWithCharacter(PlayerCharacterController character)
        {
            character.OnStanceChanged += OnStanceChanged;
            OnStanceChanged(character.IsCrouching);
        }

        void OnDestroy()
        {
            PlayerCharacterController character = FindAnyObjectByType<PlayerCharacterController>();
            if (character != null)
            {
                character.OnStanceChanged -= OnStanceChanged;
            }
        }

        void OnStanceChanged(bool crouched)
        {
            StanceImage.sprite = crouched ? CrouchingSprite : StandingSprite;
        }
    }
}