using UI;
using UnityEngine;
using UnityEngine.AI;

public class BossActivate : Interactive
{
    public BossAI bossAI;
    public GameObject explodeEffect;

    protected override void Start()
    {
        base.Start();
    }


    public void ActivateBoss()
    {
        bossAI.enabled = true;
        if (bossAI.gameObject.GetComponent<NavMeshAgent>() == null)
        { 
           
            NavMeshAgent agent= bossAI.gameObject.AddComponent<NavMeshAgent>();
            agent.speed = bossAI.speed;
        }

        
        Invoke("DestroyThis",10f);
    }

    void DestroyThis()
    {
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
}
