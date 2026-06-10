using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    public EnemyState currentState = EnemyState.Patrol;
    public float detectionRange = 15f; // 检测范�?
    public float attackRange = 2f;    // 攻击范围
    protected Transform player;
    protected float distanceToPlayer;
    protected float lastAttackTime;
    public float attackCooldown=1f;
    private Vector3 lastKnownPlayerPosition;
    protected NavMeshAgent navMeshAgent;
    //public GameObject attackArea;
    public float baseSpeed;
    public float speed;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
    }
    

    protected virtual void Update()
    {
        ChangeLogic();
        CarryOutLogic();
    }

    public void CarryOutLogic()
    {    // 状态执行逻辑
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
    }

    public void ChangeLogic()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 状态切换逻辑
        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        // && distanceToPlayer >= attackRange
        {
            currentState = EnemyState.Chase;
        }
        else if (currentState != EnemyState.Patrol)
        {
            currentState = EnemyState.Patrol;
        }
    }

   public void Patrol()
    {
        
    }
    
  public  void Chase()
    {
        if (player != null)
        {
            if (Time.frameCount % 10 == 0)
                navMeshAgent.SetDestination(player.position); // 设置目标为玩家的位置
        }
    }

   protected virtual void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            //StartCoroutine(scratch());
            lastAttackTime = Time.time;
        }
    }
   

    /*void OnDrawGizmos()
    {
        // 在场景视图中绘制射线
        if (currentState == EnemyState.Chase)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (player.position - transform.position).normalized * detectionRange);
        }
    }*/

    /*private IEnumerator scratch()
    {
        yield return new WaitForSeconds(0.5f);
        if (attackArea != null)
        {
            attackArea.SetActive(true);
        }
    }*/

    public float GetDistanceToPlayer()
    {
        return distanceToPlayer;
    }
}