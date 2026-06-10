using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MyPooler;

public class Whirlpool : MonoBehaviour,IPooledObject
{
    public float speed = 10f;
    public float lifeTime = 5f;
    private Transform playerTransform;
    private PlayerController playerController;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = playerTransform.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        Invoke("DiscardToPool", lifeTime);
    }
    private void OnDisable()
    {
        if(playerController!=null)
            playerController.moveSpeed = playerController.originalMoveSpeed;
    }

    private void Update()
    {
        if (playerController != null)
        {
            if (Time.frameCount % 10 == 0)
                agent.SetDestination(playerTransform.position ); // 设置目标为玩家的位置
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController.moveSpeed *= 0.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerController.moveSpeed = playerController.originalMoveSpeed;
        }
    }

    public void OnRequestedFromPool()
    {
        
    }

    public void DiscardToPool()
    {
        ObjectPooler.Instance.ReturnToPool("Vortex", gameObject);
    }
}