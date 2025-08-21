using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FightingGameEngine 
{
    public class GameInputManager : MonoBehaviour
    {
        public static GameInputManager Instance { get; private set; }

        [SerializeField]
        PlayerInputManager playerInputManager;

        [Header("Input Assets")]
        public InputActionAsset player1InputAsset;
        public InputActionAsset player2InputAsset;

        [Header("Player References")]
        public PlayerInput player1Input;
        public PlayerInput player2Input;

        private Dictionary<int, InputDevice> playerDevices = new Dictionary<int, InputDevice>();
        private Dictionary<int, string> playerControlSchemes = new Dictionary<int, string>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);


            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            
        }

        private void OnEnable()
        {
            // Subscribe to PlayerInputManager events
            SubscribeToPlayerInputEvents();
        }

        void OnDestroy()
        {
            // Unsubscribe from events when destroyed
            UnsubscribeFromPlayerInputEvents();
        }

        private void SubscribeToPlayerInputEvents()
        {
            Debug.Log("Subscribing to PlayerInputManager events...");
            playerInputManager = GetComponent<PlayerInputManager>();
            playerInputManager.playerJoinedEvent.AddListener(HandlePlayerJoined);
            playerInputManager.playerLeftEvent.AddListener(HandlePlayerLeft);

            Debug.Log("Subscribed to onPlayerJoined and onPlayerLeft!");
        }

        private void UnsubscribeFromPlayerInputEvents()
        {
            Debug.Log("Unsubscribing from PlayerInputManager events...");

            if (playerInputManager != null)
            {
                playerInputManager.playerJoinedEvent.RemoveListener(HandlePlayerJoined);
                playerInputManager.playerLeftEvent.RemoveListener(HandlePlayerLeft);
            }
        }

        // Called when a player joins via Unity's PlayerInputManager
        public void HandlePlayerJoined(PlayerInput playerInput)
        {
            Debug.Log($"[GameInputManager] Player {playerInput.playerIndex} joined (device: {playerInput.devices[0].displayName})");
            Debug.Log($"Player {playerInput.playerIndex} joined!");

            int playerIndex = playerInput.playerIndex;

            if (playerIndex == 0)
            {
                player1Input = playerInput;
                ApplyCustomSettingsToPlayer(playerIndex, playerInput);
            }
            else if (playerIndex == 1)
            {
                player2Input = playerInput;
                ApplyCustomSettingsToPlayer(playerIndex, playerInput);
            }
            else
            {
                Debug.LogWarning($"Unexpected player index: {playerIndex}. Only 0 and 1 are supported.");
            }
        }

        public void HandlePlayerLeft(PlayerInput playerInput)
        {
            Debug.Log($"Player {playerInput.playerIndex} left!");

            int playerIndex = playerInput.playerIndex;

            if (playerIndex == 0)
            {
                player1Input = null;
            }
            else if (playerIndex == 1)
            {
                player2Input = null;
            }
        }

        public void ApplyCustomSettingsToPlayer(int playerIndex, PlayerInput playerInput)
        {
            // Load saved settings
            LoadPlayerSettings(playerIndex);

            // Apply control scheme and device
            if (playerControlSchemes.ContainsKey(playerIndex) &&
                playerDevices.ContainsKey(playerIndex))
            {
                playerInput.SwitchCurrentControlScheme(
                    playerControlSchemes[playerIndex],
                    playerDevices[playerIndex]
                );
            }

            // Apply custom bindings
            LoadCustomBindings(playerIndex, playerInput);
        }

        public void AssignDeviceToPlayer(int playerIndex, InputDevice device, string controlScheme)
        {
            playerDevices[playerIndex] = device;
            playerControlSchemes[playerIndex] = controlScheme;

            SavePlayerSettings(playerIndex);
        }

        void SavePlayerSettings(int playerIndex)
        {
            PlayerPrefs.SetString($"Player{playerIndex}_Device", playerDevices[playerIndex].deviceId.ToString());
            PlayerPrefs.SetString($"Player{playerIndex}_Scheme", playerControlSchemes[playerIndex]);
        }

        void LoadPlayerSettings(int playerIndex)
        {
            if (PlayerPrefs.HasKey($"Player{playerIndex}_Device"))
            {
                int deviceId = int.Parse(PlayerPrefs.GetString($"Player{playerIndex}_Device"));
                string scheme = PlayerPrefs.GetString($"Player{playerIndex}_Scheme");

                // Find device by ID
                foreach (var device in InputSystem.devices)
                {
                    if (device.deviceId == deviceId)
                    {
                        playerDevices[playerIndex] = device;
                        playerControlSchemes[playerIndex] = scheme;
                        break;
                    }
                }
            }
        }

        void LoadCustomBindings(int playerIndex, PlayerInput playerInput)
        {
            // Load your custom key bindings from PlayerPrefs
            // and apply them to the player's input actions
        }
    }

}
