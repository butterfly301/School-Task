using System;
using System.Collections;
using UnityEngine;

public class ExplosiveBarrelFromEnemy : MonoBehaviour
{
    [SerializeField]  private float explosionRadius = 1f; // ��ը��Χ
  [SerializeField] private float explosionForce = 30f; // ���ͱ�ը�����?
  [SerializeField]  private float maxSpeed = 10f; // ��������ٶ�?
  [SerializeField] private AudioEventChannel audioEventChannel;
    
    public GameObject explodeEffect;
    public GameObject fireEffect;
    
    private void Start()
    {
        StartCoroutine(Explode());
    }

    private void OnEnable()
    {
        Item03Effect.OnItem03Effect += AddItem03Count;
        Inventory.OnInventoryCleared += OnInventoryCleared;
    }

    private void OnDisable()
    {
        Item03Effect.OnItem03Effect -= AddItem03Count;
        Inventory.OnInventoryCleared -= OnInventoryCleared;
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f); 
        
        // ��ⱬը��Χ�ڵĶ���?
        Collider();
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        Instantiate(fireEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void Collider()
    {
        audioEventChannel.Raise3D(SoundEvent.Explosion,transform.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.gameObject.layer!=LayerMask.NameToLayer("CanGo") )
            {
                Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                if (rb.isKinematic)
                {

                    rb.MovePosition(rb.position + forceDirection * 5f); // ��΢����
                }
                else
                {

                    rb.drag = 1.3f; // ���ӿ����������������޻���
                    rb.AddForce(forceDirection * explosionForce, ForceMode.Impulse);
                    StartCoroutine(LimitVelocity(rb)); // ��������ٶ�?
                }
                hit.GetComponent<Enemy>()?.OnHurt.Invoke();
            }
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
    
    private void AddItem03Count()
    {
        explosionRadius *= 1.25f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnInventoryCleared()
    {
        explosionRadius = 5f;
    }
}
