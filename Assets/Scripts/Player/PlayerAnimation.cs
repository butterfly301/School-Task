using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerCharacter playerCharacter;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerCharacter = GetComponent<PlayerCharacter>();
    }

    private void Start()
    {
        TriggerIdle();
    }

    private void Update()
    {
        if (playerCharacter != null && playerCharacter.isAlive)
            SetWalk();
    }

    void SetWalk()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || 
                        Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        animator.SetBool("Walk", isMoving);
    }

    public void TriggerAttack()
    {
        animator.ResetTrigger("AttackEnd");
        animator.SetTrigger("Attack");
    }

    public void TriggerAttackEnd()
    {
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Idle");
    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Death");
    }

    public void TriggerRevive()
    {
        animator.SetTrigger("Revive");
    }

    public void TriggerIdle()
    {
        animator.SetTrigger("Idle");
    }
}
