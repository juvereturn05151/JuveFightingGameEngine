using FightingGameEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControllerSceneUIManager : MonoBehaviour
{
    [SerializeField] private Image player1Sprite; 
    [SerializeField] private Image player2Sprite;

    [SerializeField] private string nextSceneName = "TestScene";

    // Update is called once per frame
    void Update()
    {
        bool isPlayer1Connected = GameInputManager.Instance.player1Input != null;
        bool isPlayer2Connected = GameInputManager.Instance.player2Input != null;

        // Update UI
        player1Sprite.color = isPlayer1Connected ? Color.white : Color.black;
        player2Sprite.color = isPlayer2Connected ? Color.white : Color.black;

        if (isPlayer1Connected && PlayerPressedStart(GameInputManager.Instance.player1Input))
        {
            GoToNextScene();
        }
        else if (isPlayer2Connected && PlayerPressedStart(GameInputManager.Instance.player2Input))
        {
            GoToNextScene();
        }
    }

    private bool PlayerPressedStart(PlayerInput playerInput)
    {
        // Replace "Start" with the actual action name you use in your InputActions
        InputAction startAction = playerInput.actions["Start"];
        return startAction != null && startAction.triggered;
    }

    private void GoToNextScene()
    {
        Debug.Log("Start pressed! Loading next scene...");
        SceneManager.LoadScene(nextSceneName);
    }
}
