using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 5f;

    [SerializeField]
    private Animator animator;

    private InputHandler inputHandler;


    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    private void Update()
    {
        Vector2 moveDir = inputHandler.CurrentInput.Direction;

        // Apply simple horizontal movement
        Vector3 delta = new Vector3(moveDir.x * walkSpeed * Time.deltaTime, 0, 0);
        transform.position += delta;

        // Flip character based on direction
        if (moveDir.x > 0)
        {
            transform.localScale = Vector3.one;
            animator.Play("WalkForward");
        }
        else if (moveDir.x < 0)
        {
            animator.Play("WalkBackward");
            //transform.localScale = new Vector3(-1, 1, 1);
        }
        else 
        {
            animator.Play("Idle");
        }
            

        // Handle attack/block/throw with MoveExecutor (not shown here)
    }
}
