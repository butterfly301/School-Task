using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using MyPooler;


public class Enemy : MonoBehaviour,IPooledObject
{
    public string poolTag;
    
    public UnityEvent OnHurt;
    public UnityEvent OnDie;
    
    public int moneyAmount;
    public float increasement=0;
    public GameObject moneyPrefab;

    public EnemyAI enemyAI;
    protected BoxCollider boxCollider;
    protected Rigidbody rigidBody;
    
    public GameObject sparkEffect;
    [SerializeField] protected AudioEventChannel channel;
    

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        boxCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        Item02Effect.OnItem02Effect += IncreaseMoneyDrop;
        Inventory.OnInventoryCleared += OnInventoryCleared;
        PlayerDeathBroadcaster.Register(this);
    }

    protected virtual void OnDisable()
    {
        PlayerDeathBroadcaster.Unregister(this);
        Item02Effect.OnItem02Effect -= IncreaseMoneyDrop;
        Inventory.OnInventoryCleared -= OnInventoryCleared;
    }

    public virtual void TakeDamage()
    {
        OnDie.Invoke();
    }
    
    // 敌人死亡时调用
    public void Die()
    {
        CoinsOut();
        MakeCorpse();
        SparkEffect();
        GameStatsManager.Instance.totalKills += 1;
        EnemyDieAudio();
        // 回收敌人到对象池
        StartCoroutine(EnemysBack(5));
    }
   public IEnumerator EnemysBack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        DiscardToPool();
    }

    public void CoinsOut()
    {
        for (int i = 0; i < moneyAmount*(1+increasement); i++)
        {
            Instantiate(moneyPrefab, transform.position, Quaternion.identity );
        }
    }

    public void MakeCorpse()
    {
        enemyAI.enabled = false;
        boxCollider.enabled = false;
        rigidBody.freezeRotation = false;
        
    }

    public void IncreaseMoneyDrop()
    {
        increasement += 0.25f;
    }

    public virtual void Set()
    {   rigidBody.velocity = Vector3.zero;
        enemyAI.enabled = true;
        boxCollider.enabled = true;
        rigidBody.freezeRotation = true;
        sparkEffect.SetActive(false);
    }

    private void SparkEffect()
    {
        sparkEffect.SetActive(true);
        StartCoroutine(DisableSparkEffect());
    }

    IEnumerator DisableSparkEffect()
    {
        yield return new WaitForSeconds(4);
        sparkEffect.SetActive(false);
    }
   
    public void DiscardToPool()
    {
        MyPooler.ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }
    
    public virtual void OnRequestedFromPool()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player")) // 检测是否带有"player"标签
            {
                DiscardToPool(); // 摧毁自己
                break; // 退出循环
            }
        }
        Set();
    }

    public virtual void OnPlayerDeath()
    {
        //Set();
        //MakeCorpse();
    }
    public void EnemyDieAudio()
    {
        int ranndNumber = UnityEngine.Random.Range(0, 5);
        switch (ranndNumber) 
        { 
            case 0:
                channel.Raise3D(SoundEvent.EnemyDie, transform.position);
                break;
            case 1:
                channel.Raise3D(SoundEvent.EnemyDie1,transform.position);
                break ;
            case 2:
                channel.Raise3D(SoundEvent.EnemyDie2,transform.position);
                break ;
            case 3:
                channel.Raise3D(SoundEvent.EenmyDie4,transform.position);
                break ;
            case 4:
                channel.Raise3D(SoundEvent.EnemyDie5,transform.position);
                break ;
        }
    }
    
    private void OnInventoryCleared()
    {
        increasement = 0;
    }
}