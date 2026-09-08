using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class JetpackCounter : MonoBehaviour
    {
        [Tooltip("Image component representing jetpack fuel")]
        public Image JetpackFillImage;

        [Tooltip("Canvas group that contains the whole UI for the jetack")]
        public CanvasGroup MainCanvasGroup;

        [Tooltip("Component to animate the color when empty or full")]
        public FillBarColorChange FillBarColorChange;

        Jetpack m_Jetpack;

        void Start()
        {
            TryBindPlayer();
        }

        void Update()
        {
            if (m_Jetpack == null)
            {
                TryBindPlayer();
                return;
            }

            MainCanvasGroup.gameObject.SetActive(m_Jetpack.IsJetpackUnlocked);

            if (m_Jetpack.IsJetpackUnlocked)
            {
                JetpackFillImage.fillAmount = m_Jetpack.CurrentFillRatio;
                FillBarColorChange.UpdateVisual(m_Jetpack.CurrentFillRatio);
            }
        }

        private void TryBindPlayer()
        {
            PlayerCharacterController localPlayer = NetworkUtils.GetLocalPlayer();

            if (localPlayer != null)
            {
                m_Jetpack = localPlayer.GetComponent<Jetpack>();
            }
        }
    }
}