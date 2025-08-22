using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FightingGameEngine
{
    // InputManager is responsible for managing input data and handling input events in the fighting game engine.
    // It can be extended to include methods for processing input, handling player controls, etc.

    public class CharacterInputManager : MonoBehaviour
    {
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

            _currentInput.input = (int)InputDefine.None;
            _currentInput.time = Time.time;

            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            if (moveInput.x < -0.5f) _currentInput.input |= (int)InputDefine.Left;
            if (moveInput.x > 0.5f) _currentInput.input |= (int)InputDefine.Right;
            if (moveInput.y < -0.5f) _currentInput.input |= (int)InputDefine.Down;
            if (moveInput.y > 0.5f) _currentInput.input |= (int)InputDefine.Up;

            if (_attackAction.triggered)
            {
                _currentInput.input |= (int)InputDefine.Attack;
            }

            if (_attackAction.triggered)
            {
                _currentInput.input |= (int)InputDefine.Attack;
            }

            if (_specialAction.triggered) 
            {
                _currentInput.input |= (int)InputDefine.Special;
            }

            if (_currentInput.input != (int)InputDefine.None)
            {
                _inputBuffer.Enqueue(_currentInput.ShallowCopy());

                //Debug.Log($"[InputManager] Buffered Input: {((InputDefine)_currentInput.input)}");

                if (_inputBuffer.Count > MaxQueueBufferSize)
                    _inputBuffer.Dequeue();
            }
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

            //if (inputDown[0] != 0)
                //Debug.Log($"[InputManager] InputDown Detected: {((InputDefine)inputDown[0])}");

            //if (inputUp[0] != 0)
                //Debug.Log($"[InputManager] InputUp Detected: {((InputDefine)inputUp[0])}");
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
    }
}

