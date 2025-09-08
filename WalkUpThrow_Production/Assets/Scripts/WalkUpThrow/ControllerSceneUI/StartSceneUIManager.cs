using FightingGameEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneUIManager : MonoBehaviour
{
    [SerializeField] Animator pressToStartAnimator;
    private bool hasStarted = false;
    private bool goToNextScene = false;
    private float animationDelay = 0.5f;
    private float countDownToNextScene;

    void Update()
    {
        if (hasStarted) 
        {
            countDownToNextScene -= Time.deltaTime;
            if (!goToNextScene && countDownToNextScene <= 0)
            {
                goToNextScene = true;
                SceneTransitionPP.Instance.LoadScene("ControllerScene");
            }
            return;
        }

        if (PlayerPressedStart(GameInputManager.Instance.player1Input) || PlayerPressedStart(GameInputManager.Instance.player2Input))
        {
            PressStart();
        }
    }

    private bool PlayerPressedStart(PlayerInput playerInput)
    {
        if (playerInput == null) return false;
        // Replace "Start" with the actual action name you use in your InputActions
        InputAction startAction = playerInput.actions["Start"];
        return startAction != null && startAction.triggered;
    }

    private void PressStart()
    {
        Debug.Log("Start pressed! Loading next scene...");
        pressToStartAnimator.SetBool("IsReady", true);
        hasStarted = true;
        countDownToNextScene = animationDelay;

        //SceneManager.LoadScene(nextSceneName);
    }
}
