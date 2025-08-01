using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FightingGameEngine
{
    // InputManager is responsible for managing input data and handling input events in the fighting game engine.
    // It can be extended to include methods for processing input, handling player controls, etc.

    public class InputManager : MonoBehaviour
    {
        // Input Action Asset reference
        [SerializeField] private InputActionAsset _inputActions;

        // Input buffers
        private Queue<InputData> _inputBuffer = new Queue<InputData>();
        private InputData _currentInput = new InputData();

        // Input Actions
        private InputAction _moveAction;
        private InputAction _attackAction;

        private void Awake()
        {
            // Set up input actions
            _moveAction = _inputActions.FindAction("Move");
            _attackAction = _inputActions.FindAction("Attack");

            // Enable actions
            _moveAction.Enable();
            _attackAction.Enable();
        }

        private void OnDestroy()
        {
            _moveAction.Disable();
            _attackAction.Disable();
        }

        private void Update()
        {
            // Process raw inputs each frame
            ProcessInputs();
        }

        private void ProcessInputs()
        {
            // Reset current input
            _currentInput.input = (int)InputDefine.None;
            _currentInput.time = Time.time;

            // Movement
            Vector2 moveInput = _moveAction.ReadValue<Vector2>();
            if (moveInput.x < -0.5f) _currentInput.input |= (int)InputDefine.Left;
            if (moveInput.x > 0.5f) _currentInput.input |= (int)InputDefine.Right;

            // Attack
            if (_attackAction.triggered)
                _currentInput.input |= (int)InputDefine.Attack;

            // Add to buffer if there's any input
            if (_currentInput.input != (int)InputDefine.None)
                _inputBuffer.Enqueue(_currentInput.ShallowCopy());
        }

        public bool GetInput(InputDefine input)
        {
            return (_currentInput.input & (int)input) != 0;
        }

        public bool GetInputDown(InputDefine input)
        {
            foreach (InputData data in _inputBuffer)
            {
                if ((data.input & (int)input) != 0)
                    return true;
            }
            return false;
        }

        public void ClearBuffer()
        {
            _inputBuffer.Clear();
        }

        // For the new Input System UI
        public void OnMove(InputAction.CallbackContext context)
        {
            // Can be used for UI-driven input if needed
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            // Can be used for UI-driven input if needed
        }
    }
}

