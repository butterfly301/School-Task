using System;
using UnityEngine;
using UnityEngine.AI;

public class ItemEffectOnEnemy : MonoBehaviour
{
    
    private float originalSpeed;
    
    private bool enable01;
    private int item01Count;
    
    
    private bool enable03;
    
    
    public GameObject blockingStripPrefab;
    public GameObject ExplosiveeBarrelPrefab;

    protected virtual void OnEnable()
    {
        item01Count = SaveManager.Instance != null ? SaveManager.Instance.GetPersistentItemCount(1) : 0;
        enable01 = item01Count > 0;
        enable03 = SaveManager.Instance != null && SaveManager.Instance.GetPersistentItemCount(3) > 0;
        Item01Effect.OnItem01Effect += AddItem01Count;
        Item01Effect.OnItem01Effect += TriggerItem01Effect;
        Item03Effect.OnItem03Effect += TriggerItem03Effect;
        Inventory.OnInventoryCleared += OnInventoryCleared;
    }

    protected virtual void OnDisable()
    {
        Item01Effect.OnItem01Effect -= AddItem01Count;
        Item01Effect.OnItem01Effect -= TriggerItem01Effect;
        Item03Effect.OnItem03Effect -= TriggerItem03Effect;
        Inventory.OnInventoryCleared -= OnInventoryCleared;
    }

    private void TriggerItem03Effect()
    {
        enable03 = true;
    }

    public void CheckItemEffect()
    {
        if (enable01)
        {
            Instantiate(blockingStripPrefab, transform.position, Quaternion.identity);
        }

        if (enable03)
        {
            Instantiate(ExplosiveeBarrelPrefab, transform.position, Quaternion.identity);
        }
        
    }
    
    private void TriggerItem01Effect()
    {
        enable01 = true;
    }
    
    

    public void Strip()
    {
        var agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        for(int i=1;i<item01Count+1;i++)
            agent.speed *= 0.75f;
    }

    public void StopStrip()
    {
        var agent = GetComponent<NavMeshAgent>();
        agent.speed = originalSpeed;
    }
    
    private void AddItem01Count()
    {
        item01Count++;
    }
    
    private void OnInventoryCleared()
    {
        enable01 = false;
        item01Count = 0;
        enable03 = false;
    }
}
