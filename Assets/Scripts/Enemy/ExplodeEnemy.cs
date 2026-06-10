using System.Collections;
using UI;
using UnityEngine;

public class ExplodeEnemy : Enemy
{
    [SerializeField] private float explosionRadius;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float explosionDelay;
    [SerializeField] private float drag=1f;
    private float distance;
    private Transform player;
    private bool isCountingDown = false; 
    public bool isEnemyAlive=true;
    public bool isEnemyBoom=false;
    
   [SerializeField] private AudioEventChannel channelm;
    
    public SoundManager soundManager;
    
    public GameObject explodeEffect;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
       
    }

    // Update is called once per frame
    void Update()
    {
        DistanceMonitor();
    }
    
    public void DistanceMonitor()
    {
        float distance = Vector3.Distance(transform.position,player.transform. position);
       // print(player.transform.position);

        if (distance <= detectionRadius && !isCountingDown )
        {
            Set();
            boxCollider.enabled = true;
            StartCoroutine(StartExplosionCountdown());
        }
        else if (distance > detectionRadius|| isCountingDown==false)
        {
            StopCoroutine(StartExplosionCountdown());
            isCountingDown = false;
        }
    }
    IEnumerator StartExplosionCountdown()
    {
        isCountingDown = true;
        channel.Raise3D(SoundEvent.ExplosionWarning,transform.position);
        yield return new WaitForSeconds(explosionDelay);
        if (isEnemyAlive)
        {
            //  print(finalDistance);
            Explosion();
            
            channelm.Raise3D(SoundEvent.Explosion,transform.position);
           
        }
        else 
        {
            isCountingDown = false; 
        }
       
    }
    
    public void Explosion()
    {
        FightUIManager.Instance.visionPanel.onWarningDisable();
        StopWarning();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player")||isEnemyAlive)
            {
                isEnemyBoom = true;
                
                if (isEnemyAlive)
                {
                    ExpoplosionEffet();
                    isEnemyAlive = true;
                }
            }
            Vector3 direction =  hitCollider.transform.position-transform.position;
            var rb= hitCollider.GetComponent<Rigidbody>();
            
            if (rb != null )
            {   
                if(hitCollider.gameObject.layer != LayerMask.NameToLayer("CanGo"))
                {
                    
                    rb.drag = drag;
                    rb.velocity = direction.normalized * rb.mass;
                }
              
            }

            hitCollider.GetComponent<PlayerCharacter>()?.OnHurt.Invoke();
            hitCollider.GetComponent<Enemy>()?.OnHurt.Invoke();
        }
        StartCoroutine(EnemysBack(5));


    }
    private void ExpoplosionEffet()
    {
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
    }
    public void EnemyDie()
    {
        StopWarning();
        Set();
        isEnemyAlive = false;
        if (isEnemyBoom == false)
        {
            CoinsOut();
            
        }
        
        StartCoroutine(EnemysBack(1.7f));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
        Gizmos.DrawWireSphere (transform.position, detectionRadius);
    }

    public void WarningDisable()
    {
        FightUIManager.Instance.visionPanel.onWarningDisable();
    }
    public override void OnPlayerDeath()
    {
        base.OnPlayerDeath();
       
        Set();
    }
    public void StopWarning()
    {
        FindObjectOfType<SoundManager>().StopSound(SoundEvent.ExplosionWarning);


    }
}
