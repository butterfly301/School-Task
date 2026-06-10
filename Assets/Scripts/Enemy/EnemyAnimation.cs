using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    private EnemyAI enemyAI;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        enemyAI = GetComponentInParent<EnemyAI>();
    }

    private void Update()
    {
        if (enemyAI.enabled)
        {
            switch (enemyAI.currentState)
            {
                case EnemyAI.EnemyState.Patrol:
                    animator.SetBool("Chase", false);
                    break;
                case EnemyAI.EnemyState.Attack:
                    animator.SetTrigger("Attack");
                    break;
                case EnemyAI.EnemyState.Chase:
                    animator.SetBool("Chase", true);
                    break;
            }
        }
    }

    public void DeathAnimation()
    {
        animator.Play("Death");
    }
}
