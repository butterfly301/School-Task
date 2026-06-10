using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Events;


public class ExplosiveBarrel : MonoBehaviour
{
  [SerializeField]  private float explosionRadius = 5f; // ��ը��Χ
  
  [SerializeField]  private float maxSpeed = 10f; // ��������ٶ�?
    [SerializeField] private AudioEventChannel AudioEventChannel;
    private bool isTriggered = false;
    public GameObject explodeEffect;
    public GameObject fireEffect;
    public UnityEvent OnIgnite;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            OnIgnite.Invoke();
            FightUIManager.Instance.visionPanel.onWarningEnable(transform);
            StartCoroutine(Explode());
        }

        if (other.CompareTag("Bullet") && !isTriggered)
        {
            isTriggered = true;
            OnIgnite.Invoke();
            FightUIManager.Instance.visionPanel.onWarningEnable(transform);
            StartCoroutine(Explode());
        }
    }

    private void Update()
    {
        if(isTriggered)
            FightUIManager.Instance.visionPanel.onWarningKeep(transform);
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(1f); 
        // ��ⱬը��Χ�ڵĶ���?
        Collider();
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        AudioEventChannel.Raise3D(SoundEvent.Explosion,transform.position);
        Instantiate(fireEffect, transform.position,Quaternion.identity );
        FightUIManager.Instance.visionPanel.onWarningDisable();
        isTriggered = false;
        Destroy(gameObject);
    }

    private void Collider()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            Vector3 direction =  hit.transform.position-transform.position;
            if (rb != null )
            {   
                if(hit.gameObject.layer != LayerMask.NameToLayer("CanGo"))
                {
                    rb.velocity = direction.normalized * rb.mass;
                }
                hit.GetComponent<PlayerCharacter>()?.OnHurt.Invoke();
                hit.GetComponent<Enemy>()?.OnHurt.Invoke();
            }

            /*if (rb != null&&hit.gameObject.layer!=LayerMask.NameToLayer("CanGo") )
            {
                Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                if (rb.isKinematic)
                {

                    rb.MovePosition(rb.position + forceDirection * 5f); // ��΢����
                }
                else
                {
                    
                    rb.AddForce(forceDirection * explosionForce, ForceMode.Impulse);
                    StartCoroutine(LimitVelocity(rb)); // ��������ٶ�?
                }
                hit.GetComponent<PlayerCharacter>()?.OnHurt.Invoke();
                hit.GetComponent<Enemy>()?.OnHurt.Invoke();
            }*/
        }
    }

    private IEnumerator LimitVelocity(Rigidbody rb)
    {
        while (rb != null && rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
