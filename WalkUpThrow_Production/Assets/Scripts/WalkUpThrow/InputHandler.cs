using FightinGameEngine;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private SimpleInput _currentInput;
    public SimpleInput CurrentInput => _currentInput;

    private SimpleFightingGameInput controls;

    private void Awake()
    {
        controls = new SimpleFightingGameInput();

        controls.Gameplay.Move.performed += ctx => {
            Vector2 raw = ctx.ReadValue<Vector2>();
            _currentInput.Direction = raw.normalized;
        };
        controls.Gameplay.Move.canceled += ctx => {
            _currentInput.Direction = Vector2.zero;
        };

        controls.Gameplay.Attack.performed += ctx => _currentInput.Attack = true;
        controls.Gameplay.Attack.canceled += ctx => _currentInput.Attack = false;

        controls.Gameplay.Block.performed += ctx => _currentInput.Block = true;
        controls.Gameplay.Block.canceled += ctx => _currentInput.Block = false;

        controls.Gameplay.Throw.performed += ctx => _currentInput.Throw = true;
        controls.Gameplay.Throw.canceled += ctx => _currentInput.Throw = false;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void LateUpdate()
    {
        // Reset one-shot inputs each frame if you want press-only behavior
        _currentInput.Attack = false;
        _currentInput.Throw = false;
    }
}