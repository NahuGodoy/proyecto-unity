using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Photon.Pun; // Para verificar el estado de red

namespace Unity.FPS.UI
{
    public class InGameMenuManager : MonoBehaviour
    {
        [Tooltip("Root GameObject of the menu used to toggle its activation")]
        public GameObject MenuRoot;


        [Tooltip("Slider component for look sensitivity")]
        public Slider LookSensitivitySlider;

        [Tooltip("Slider component for volume")]
        public Slider VolumeSlider;

        [Tooltip("Toggle component for invincibility")]
        public Toggle InvincibilityToggle;

        [Tooltip("Toggle component for framerate display")]
        public Toggle FramerateToggle;

        [Tooltip("GameObject for the controls")]
        public GameObject ControlImage;

        PlayerInputHandler m_PlayerInputsHandler;
        Health m_PlayerHealth;
        FramerateCounter m_FramerateCounter;

        private InputAction m_SubmitAction;
        private InputAction m_CancelAction;
        private InputAction m_NavigateAction;
        private InputAction m_MenuAction;

        private bool m_IsBound = false;

        private float m_MasterVolume = 1f;

        void Start()
        {
            MenuRoot.SetActive(false);

            m_FramerateCounter = FindAnyObjectByType<FramerateCounter>();

            // Configuración inicial independiente del jugador

            if (VolumeSlider != null)
            {
                VolumeSlider.value = AudioUtility.GetMasterVolume();
                /* VolumeSlider.value = m_MasterVolume; */
                VolumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            if (FramerateToggle != null && m_FramerateCounter != null)
            {
                FramerateToggle.isOn = m_FramerateCounter.UIText.gameObject.activeSelf;
                FramerateToggle.onValueChanged.AddListener(OnFramerateCounterChanged);
            }

            // Input System Actions
            m_SubmitAction = InputSystem.actions.FindAction("UI/Submit");
            m_CancelAction = InputSystem.actions.FindAction("UI/Cancel");
            m_NavigateAction = InputSystem.actions.FindAction("UI/Navigate");
            m_MenuAction = InputSystem.actions.FindAction("UI/Menu");

            m_SubmitAction?.Enable();
            m_CancelAction?.Enable();
            m_NavigateAction?.Enable();
            m_MenuAction?.Enable();

            // Intentar enlazar con el personaje local al iniciar
            TryBindLocalPlayer();
        }

        void Update()
        {
            // Si el jugador aún no se ha instanciado en red, reintentamos el enlace
            if (!m_IsBound)
            {
                TryBindLocalPlayer();
            }

            // Lock cursor when clicking outside of menu
            if (!MenuRoot.activeSelf && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if ((m_MenuAction != null && m_MenuAction.WasPressedThisFrame())
                || (MenuRoot.activeSelf && m_CancelAction != null && m_CancelAction.WasPressedThisFrame()))
            {
                if (ControlImage.activeSelf)
                {
                    ControlImage.SetActive(false);
                    return;
                }

                SetPauseMenuActivation(!MenuRoot.activeSelf);
            }

            if (m_NavigateAction != null && m_NavigateAction.ReadValue<Vector2>().y != 0)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    if (LookSensitivitySlider != null)
                        LookSensitivitySlider.Select();
                }
            }
        }

        private void TryBindLocalPlayer()
        {
            PlayerCharacterController localPlayer = NetworkUtils.GetLocalPlayer();

            if (localPlayer == null) return;

            m_PlayerInputsHandler = localPlayer.GetComponent<PlayerInputHandler>();
            m_PlayerHealth = localPlayer.GetComponent<Health>();

            if (m_PlayerInputsHandler != null && m_PlayerHealth != null)
            {
                // Enlazar sensibilidad de mouse
                if (LookSensitivitySlider != null)
                {
                    LookSensitivitySlider.value = m_PlayerInputsHandler.LookSensitivity;
                    LookSensitivitySlider.onValueChanged.RemoveAllListeners();
                    LookSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
                }

                // Enlazar modo invencible
                if (InvincibilityToggle != null)
                {
                    InvincibilityToggle.isOn = m_PlayerHealth.Invincible;
                    InvincibilityToggle.onValueChanged.RemoveAllListeners();
                    InvincibilityToggle.onValueChanged.AddListener(OnInvincibilityChanged);
                }

                m_IsBound = true;
            }
        }

        public void ClosePauseMenu()
        {
            SetPauseMenuActivation(false);
        }

        void SetPauseMenuActivation(bool active)
        {
            MenuRoot.SetActive(active);

            if (MenuRoot.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Solo pausamos el juego si estamos en modo Singleplayer/Offline
                if (!PhotonNetwork.IsConnected || PhotonNetwork.OfflineMode)
                {
                    Time.timeScale = 0f;
                }

               EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1f;
                AudioUtility.SetMasterVolume(m_MasterVolume);
            }
        }

        void OnMouseSensitivityChanged(float newValue)
        {
            if (m_PlayerInputsHandler != null)
                m_PlayerInputsHandler.LookSensitivity = newValue;
        }


        public void OnVolumeChanged(float newValue)
        {
            m_MasterVolume = newValue;
            AudioUtility.SetMasterVolume(newValue);
        }

        void OnInvincibilityChanged(bool newValue)
        {
            if (m_PlayerHealth != null)
                m_PlayerHealth.Invincible = newValue;
        }

        void OnFramerateCounterChanged(bool newValue)
        {
            if (m_FramerateCounter != null)
                m_FramerateCounter.UIText.gameObject.SetActive(newValue);
        }

        public void OnShowControlButtonClicked(bool show)
        {
            ControlImage.SetActive(show);
        }
    }
}