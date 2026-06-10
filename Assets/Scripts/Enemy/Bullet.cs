using System;
using System.Collections;
using UnityEngine;
using MyPooler;

public class Bullet : MonoBehaviour,IPooledObject
{
    public string poolTag;
    public string homingTag;
    private Rigidbody rb;
    public float speed;
    public LayerMask collideLayerMask;
    public GameObject explodeEffect;
    [SerializeField] private AudioEventChannel AudioEventChannel;
    
    // 新增跟踪相关变量
    public bool isHoming; // 是否启用跟踪
    public float maxHomingAngle = 30f; // 最大转向角�?�?
    public float homingDelay = 0.5f; // 发射后开始跟踪的延迟时间
    private Transform target; // 跟踪目标(玩家)
    private float homingTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // 初始化时查找玩家目标
        if (isHoming)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DestroyAfterDelay(10f));
    }

    private void Update()
    {
        if (!isHoming || target == null)
        {
            // 普通子弹行�?
            rb.velocity = transform.forward * speed;
            return;
        }
        
        // 跟踪逻辑
        homingTimer += Time.deltaTime;
        if (homingTimer >= homingDelay)
        {
            // 计算目标方向
            Vector3 targetDirection = (target.position - transform.position).normalized;
            
            // 计算当前方向与目标方向之间的角度
            float angle = Vector3.Angle(transform.forward, targetDirection);
            
            // 限制转向角度
            float turnAngle = Mathf.Min(angle, maxHomingAngle * Time.deltaTime);
            
            // 计算新的方向
            Vector3 newDirection = Vector3.RotateTowards(
                transform.forward, 
                targetDirection, 
                turnAngle * Mathf.Deg2Rad, 
                0f);
            
            // 更新子弹方向和速度
            transform.rotation = Quaternion.LookRotation(newDirection);
            rb.velocity = newDirection * speed;
        }
        else
        {
            // 延迟期间保持初始方向
            rb.velocity = transform.forward * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((collideLayerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            Instantiate(explodeEffect, transform.position, transform.rotation);
            AudioEventChannel.Raise3D(SoundEvent.Explosion,transform.position);
            if(other.CompareTag("Player"))
                other.GetComponent<PlayerCharacter>()?.OnHurt.Invoke();
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<Enemy>()?.OnHurt.Invoke();
                other.GetComponent<SecondBossDoor>()?.OnHurt.Invoke();
            }
                
            DiscardToPool();
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DiscardToPool();
        //MyPooler.ObjectPooler.Instance.ReturnToPool(poolTag,gameObject);
    }

    public void OnRequestedFromPool()
    {
        
    }

    public void DiscardToPool()
    {
        MyPooler.ObjectPooler.Instance.ReturnToPool(poolTag, this.gameObject);
    }
}
