using System.Collections;
using MyPooler;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IPooledObject
{
    public string poolTag;

    public UnityEvent OnHurt;
    public UnityEvent OnDie;

    [Header("Stats")]
    [Min(1)] public int maxHealth = 3;
    public int moneyAmount;
    public float increasement = 0;
    public GameObject moneyPrefab;

    [Header("Components")]
    public EnemyAI enemyAI;
    protected BoxCollider boxCollider;
    protected Rigidbody rigidBody;
    public GameObject sparkEffect;
    [SerializeField] protected AudioEventChannel channel;

    private EnemyHealthBar healthBar;
    protected int currentHealth;

    protected virtual void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        boxCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();
        healthBar = GetComponent<EnemyHealthBar>();
        if (healthBar == null)
        {
            healthBar = gameObject.AddComponent<EnemyHealthBar>();
        }
    }

    protected virtual void OnEnable()
    {
        increasement = SaveManager.Instance != null ? SaveManager.Instance.GetPersistentItemCount(2) * 0.25f : 0f;
        ResetHealth(maxHealth);
        healthBar.SetVisible(true);
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
        if (ApplyDamage(1))
        {
            OnDie.Invoke();
        }
    }

    protected bool ApplyDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        RefreshHealthBar();
        return currentHealth <= 0;
    }

    protected void ResetHealth(int healthValue)
    {
        maxHealth = Mathf.Max(1, healthValue);
        currentHealth = maxHealth;
        RefreshHealthBar();
    }

    protected void RefreshHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Die()
    {
        CoinsOut();
        MakeCorpse();
        SparkEffect();
        healthBar.SetVisible(false);
        GameStatsManager.Instance.totalKills += 1;
        EnemyDieAudio();
        StartCoroutine(EnemysBack(5));
    }

    public IEnumerator EnemysBack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        DiscardToPool();
    }

    public void CoinsOut()
    {
        for (int i = 0; i < moneyAmount * (1 + increasement); i++)
        {
            Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        }
    }

    public void MakeCorpse()
    {
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        if (rigidBody != null)
        {
            rigidBody.freezeRotation = false;
        }
    }

    public void IncreaseMoneyDrop()
    {
        increasement += 0.25f;
    }

    public virtual void Set()
    {
        ResetHealth(maxHealth);
        healthBar.SetVisible(true);

        if (rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.freezeRotation = true;
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }

        if (sparkEffect != null)
        {
            sparkEffect.SetActive(false);
        }
    }

    private void SparkEffect()
    {
        if (sparkEffect == null)
            return;

        sparkEffect.SetActive(true);
        StartCoroutine(DisableSparkEffect());
    }

    private IEnumerator DisableSparkEffect()
    {
        yield return new WaitForSeconds(4f);
        if (sparkEffect != null)
        {
            sparkEffect.SetActive(false);
        }
    }

    public void DiscardToPool()
    {
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }

    public virtual void OnRequestedFromPool()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                DiscardToPool();
                break;
            }
        }

        Set();
    }

    public virtual void OnPlayerDeath()
    {
    }

    public void EnemyDieAudio()
    {
        int randomNumber = Random.Range(0, 5);
        switch (randomNumber)
        {
            case 0:
                channel.Raise3D(SoundEvent.EnemyDie, transform.position);
                break;
            case 1:
                channel.Raise3D(SoundEvent.EnemyDie1, transform.position);
                break;
            case 2:
                channel.Raise3D(SoundEvent.EnemyDie2, transform.position);
                break;
            case 3:
                channel.Raise3D(SoundEvent.EenmyDie4, transform.position);
                break;
            case 4:
                channel.Raise3D(SoundEvent.EnemyDie5, transform.position);
                break;
        }
    }

    private void OnInventoryCleared()
    {
        increasement = 0;
    }
}
