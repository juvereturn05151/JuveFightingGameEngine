using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FightingGameEngine
{
    // InputManager is responsible for managing input data and handling input events in the fighting game engine.
    // It can be extended to include methods for processing input, handling player controls, etc.

    public class CharacterInputManager : MonoBehaviour
    {
        [SerializeField] private bool _isPlayerOne = true;

        [SerializeField]
        private PlayerInput _playerInput;

        // Input Actions
        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _specialAction;

        // Raw input per frame
        private InputData _currentInput = new InputData();
        public InputData CurrentInput => _currentInput;

        // Queue buffer for command detection (Hadoken, etc.)
        private Queue<InputData> _inputBuffer = new Queue<InputData>();
        public Queue<InputData> InputBuffer => _inputBuffer;
        private const int MaxQueueBufferSize = 60;

        private List<InputDefine> _currentMotion = new List<InputDefine>();
        private const float MotionTimeout = 0.5f; // Half second timeout for motion input
        private float _lastInputTime;

        // Fixed-length frame-by-frame input history
        private const int HistorySize = 10;
        private int[] input = new int[HistorySize];
        private int[] inputDown = new int[HistorySize];
        private int[] inputUp = new int[HistorySize];

        public int[] InputHistory => input;
        public int[] InputDownHistory => inputDown;
        public int[] InputUpHistory => inputUp;

        private void Awake()
        {

        }

        public void AssignInput(PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _moveAction = _playerInput.actions["Move"];
            _attackAction = _playerInput.actions["Attack"];
            _specialAction = _playerInput.actions["Special"];

            _moveAction.Enable();
            _attackAction.Enable();
            _specialAction.Enable();
        }

        private void OnDestroy()
        {
            _moveAction.Disable();
            _attackAction.Disable();

            Debug.Log("[InputManager] Input actions disabled.");
        }


        private void Update()
        {
            ProcessInputs();
            UpdateInputHistory(_currentInput);

            // DEBUG: Show current frame's input
            //Debug.Log($"[InputManager] Frame Input: {((InputDefine)_currentInput.input)} @ {Time.time:F2}");

            // Optional: Display history at interval or key press
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("[InputManager] Input History Dump:");
                for (int i = 0; i < HistorySize; i++)
                {
                    Debug.Log($"  [{i}] Input={((InputDefine)input[i])}, Down={((InputDefine)inputDown[i])}, Up={((InputDefine)inputUp[i])}");
                }
            }
        }

        private void ProcessInputs()
        {
            if (_playerInput == null)
            {
                return;
            }

            // Read current input
            int newInput = (int)InputDefine.None;
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            if (moveInput.x < -0.5f) newInput |= (int)InputDefine.Left;
            if (moveInput.x > 0.5f) newInput |= (int)InputDefine.Right;
            if (moveInput.y < -0.5f) newInput |= (int)InputDefine.Down;
            if (moveInput.y > 0.5f) newInput |= (int)InputDefine.Up;

            // Use ReadValue instead of triggered for continuous button state
            float attackValue = _attackAction.ReadValue<float>();
            float specialValue = _specialAction.ReadValue<float>();

            if (attackValue > 0.5f) newInput |= (int)InputDefine.Attack;
            if (specialValue > 0.5f) newInput |= (int)InputDefine.Special;

            // Check if input changed from previous frame
            if (newInput != _currentInput.input)
            {
                _currentInput.input = newInput;
                _currentInput.time = Time.time;
                _currentInput.frame = Time.frameCount;
                _currentInput.duration = 1;

                _inputBuffer.Enqueue(_currentInput.ShallowCopy());
            }
            else if (_inputBuffer.Count > 0)
            {
                // Update duration of the last input
                RebuildBufferWithUpdatedDuration();
            }

            CleanupOldInputs();
        }



        private void UpdateInputHistory(InputData inputData)
        {
            for (int i = HistorySize - 1; i >= 1; i--)
            {
                input[i] = input[i - 1];
                inputDown[i] = inputDown[i - 1];
                inputUp[i] = inputUp[i - 1];
            }

            input[0] = inputData.input;
            inputDown[0] = (input[0] ^ input[1]) & input[0];   // Buttons pressed this frame
            inputUp[0] = (input[0] ^ input[1]) & ~input[0];   // Buttons released this frame
        }

        public bool GetInput(InputDefine inputCheck)
        {
            return (_currentInput.input & (int)inputCheck) != 0;
        }

        public bool GetInputDown(InputDefine inputCheck)
        {
            return (inputDown[0] & (int)inputCheck) != 0;
        }

        public bool GetInputUp(InputDefine inputCheck)
        {
            return (inputUp[0] & (int)inputCheck) != 0;
        }

        public void ClearBuffer()
        {
            _inputBuffer.Clear();
            Debug.Log("[InputManager] Input buffer cleared.");
        }

        public bool CheckHadokenMotion()
        {
            if (_inputBuffer.Count < 4) return false;

            var bufferArray = _inputBuffer.ToArray();

            // Look for the pattern: Down - DownRight - Right - Attack
            // We need to check in chronological order (oldest to newest)
            for (int startIndex = 0; startIndex <= bufferArray.Length - 4; startIndex++)
            {
                InputData downInput = bufferArray[startIndex];
                InputData downRightInput = bufferArray[startIndex + 1];
                InputData rightInput = bufferArray[startIndex + 2];
                InputData attackInput = bufferArray[startIndex + 3];

                // Check if inputs are within reasonable time window (20 frames ~0.33s)
                if (attackInput.frame - downInput.frame > 20) continue;

                // Debug: Show what we're checking
                string debugPattern = $"{InputToSymbols(downInput.input)} - " +
                                     $"{InputToSymbols(downRightInput.input)} - " +
                                     $"{InputToSymbols(rightInput.input)} - " +
                                     $"{InputToSymbols(attackInput.input)}";
                Debug.Log($"Checking pattern: {debugPattern} (Frames: {downInput.frame}-{attackInput.frame})");

                // Check for the quarter-circle forward pattern
                bool hasDown = (downInput.input & (int)InputDefine.Down) != 0;
                bool hasDownRight = (downRightInput.input & (int)InputDefine.Down) != 0 &&
                                   (downRightInput.input & (int)InputDefine.Right) != 0;
                bool hasRight = (rightInput.input & (int)InputDefine.Right) != 0;
                bool hasAttack = (attackInput.input & (int)InputDefine.Attack) != 0;

                if (hasDown && hasDownRight && hasRight && hasAttack)
                {
                    Debug.Log($" HADOKEN DETECTED! Pattern: {debugPattern}");
                    return true;
                }
            }

            return false;
        }

        private void OnGUI()
        {
            if (!Application.isEditor) return;

            // Set up styles
            GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.fontSize = 14;

            GUIStyle inputStyle = new GUIStyle(GUI.skin.label);
            inputStyle.fontSize = 12;

            // Determine position and color based on player side
            float xPos = _isPlayerOne ? 10f : Screen.width - 310f;
            Color textColor = _isPlayerOne ? Color.cyan : Color.yellow;

            headerStyle.normal.textColor = textColor;
            inputStyle.normal.textColor = textColor;

            float yPos = 40f;

            // Player identifier header
            string playerText = _isPlayerOne ? "PLAYER 1 INPUT" : "PLAYER 2 INPUT";
            GUI.Label(new Rect(xPos, 10, 300, 20), playerText, headerStyle);

            // Display current input with symbols
            string currentSymbols = InputToSymbols(_currentInput.input);
            string currentDisplay = $"Current: {currentSymbols}";
            if (_currentInput.duration > 1 && currentSymbols != "N")
                currentDisplay += $" ×{_currentInput.duration}";
            GUI.Label(new Rect(xPos, 30, 300, 20), currentDisplay, inputStyle);

            // Display buffer contents
            GUI.Label(new Rect(xPos, 50, 400, 20), "=== INPUT HISTORY ===", headerStyle);
            yPos = 70f;

            int count = 0;
            var bufferArray = _inputBuffer.ToArray();

            // Display newest first (most recent at top)
            for (int i = bufferArray.Length - 1; i >= 0; i--)
            {
                var input = bufferArray[i];
                string inputSymbols = InputToSymbols(input.input);

                if (inputSymbols == "N") continue; // Skip "None" entries for cleaner display

                string inputStr = $"{inputSymbols}";
                if (input.duration > 1) inputStr += $" x{Mathf.Min(input.duration, 99)}";

                // Add frame info for debugging
                inputStr += $" (F{input.frame})";

                GUI.Label(new Rect(xPos, yPos, 300, 20), inputStr, inputStyle);
                yPos += 18f;
                count++;

                if (count > 12) break; // Show limited entries for cleanliness
            }

            // Display motion detection result
            yPos += 10f;
            bool hadokenDetected = CheckHadokenMotion();
            string resultText = hadokenDetected ? " HADOKEN" : "No special";
            GUI.Label(new Rect(xPos, yPos, 300, 20), resultText, headerStyle);
        }

        private void RebuildBufferWithUpdatedDuration()
        {
            if (_inputBuffer.Count == 0) return;

            // Convert queue to list so we can modify the last element
            var bufferList = new List<InputData>(_inputBuffer);
            var lastInput = bufferList[bufferList.Count - 1];
            lastInput.duration = Time.frameCount - lastInput.frame + 1;
            bufferList[bufferList.Count - 1] = lastInput;

            // Rebuild the queue
            _inputBuffer.Clear();
            foreach (var input in bufferList)
            {
                _inputBuffer.Enqueue(input);
            }
        }

        private void CleanupOldInputs()
        {
            // Remove inputs older than MaxQueueBufferSize frames
            while (_inputBuffer.Count > 0 &&
                   Time.frameCount - _inputBuffer.Peek().frame > MaxQueueBufferSize)
            {
                _inputBuffer.Dequeue();
            }
        }

        private string InputToSymbols(int inputValue)
        {
            // Check for diagonals first
            if ((inputValue & (int)InputDefine.Down) != 0 && (inputValue & (int)InputDefine.Left) != 0)
                return "Down+Left"; // Down+Left
            if ((inputValue & (int)InputDefine.Down) != 0 && (inputValue & (int)InputDefine.Right) != 0)
                return "Down+Right"; // Down+Right
            if ((inputValue & (int)InputDefine.Up) != 0 && (inputValue & (int)InputDefine.Left) != 0)
                return "Up+Left"; // Up+Left
            if ((inputValue & (int)InputDefine.Up) != 0 && (inputValue & (int)InputDefine.Right) != 0)
                return "Up+Right"; // Up+Right

            // Check for cardinal directions
            if ((inputValue & (int)InputDefine.Left) != 0) return "Left";
            if ((inputValue & (int)InputDefine.Right) != 0) return "Right";
            if ((inputValue & (int)InputDefine.Down) != 0) return "Down";
            if ((inputValue & (int)InputDefine.Up) != 0) return "Up";

            // Check for buttons
            string buttons = "";
            if ((inputValue & (int)InputDefine.Attack) != 0) buttons += "P";
            if ((inputValue & (int)InputDefine.Special) != 0) buttons += "K";

            // Check for button+direction combinations
            if (!string.IsNullOrEmpty(buttons))
            {
                string direction = "";
                if ((inputValue & (int)InputDefine.Left) != 0) direction = "Left";
                if ((inputValue & (int)InputDefine.Right) != 0) direction = "Right";
                if ((inputValue & (int)InputDefine.Down) != 0) direction = "Down";
                if ((inputValue & (int)InputDefine.Up) != 0) direction = "Up";

                if (!string.IsNullOrEmpty(direction))
                    return direction + "+" + buttons;

                return buttons;
            }

            return "N"; // None
        }

    }
}

