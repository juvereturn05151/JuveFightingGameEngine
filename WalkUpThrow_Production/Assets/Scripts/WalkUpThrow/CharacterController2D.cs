using FightinGameEngine;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 5f;

    [SerializeField]
    private Animator animator;

    [SerializeField] private MoveData moveData;
    [SerializeField] private HitBoxManager hitboxManager;

    private InputHandler inputHandler;

    private bool isAttacking = false;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
    }

    private void Update()
    {
        // Prioritize attack over movement
        if (!isAttacking && inputHandler.CurrentInput.Attack)
        {
            Attack();
            return;
        }

        Vector2 moveDir = inputHandler.CurrentInput.Direction;

        // Apply simple horizontal movement
        Vector3 delta = new Vector3(moveDir.x * walkSpeed * Time.deltaTime, 0, 0);
        transform.position += delta;

        // Flip character and handle walk animations
        //if (moveDir.x > 0)
        //{
        //    transform.localScale = Vector3.one;
        //    animator.Play("WalkForward");
        //}
        //else if (moveDir.x < 0)
        //{
        //    //transform.localScale = new Vector3(-1, 1, 1);
        //    animator.Play("WalkBackward");
        //}
        //else
        //{
        //    animator.Play("Idle");
        //}
    }

    private void Attack()
    {
        isAttacking = true;
        animator.Play("Attack");

        // You can also trigger events on animation using Animation Events
        // to instantiate a hitbox or return to idle

        // Optionally: use a coroutine to control timing
        Invoke(nameof(EndAttack), 0.5f); // 0.5s attack duration (tweak as needed)
    }

    private void EndAttack()
    {
        isAttacking = false;
    }
}
