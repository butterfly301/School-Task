using System.Collections;
using UnityEngine;

public class ExplosiveBarrelFromEnemy : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 1f;
    [SerializeField] private float explosionForce = 30f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private AudioEventChannel audioEventChannel;

    public GameObject explodeEffect;
    public GameObject fireEffect;

    private float baseExplosionRadius;

    private void Start()
    {
        StartCoroutine(Explode());
    }

    private void OnEnable()
    {
        if (baseExplosionRadius <= 0f)
        {
            baseExplosionRadius = explosionRadius;
        }

        int item03Count = SaveManager.Instance != null ? SaveManager.Instance.GetPersistentItemCount(3) : 0;
        explosionRadius = baseExplosionRadius * Mathf.Pow(1.25f, item03Count);

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

        ApplyExplosion();
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        Instantiate(fireEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ApplyExplosion()
    {
        audioEventChannel.Raise3D(SoundEvent.Explosion, transform.position);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null && hit.gameObject.layer != LayerMask.NameToLayer("CanGo"))
            {
                Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                if (rb.isKinematic)
                {
                    rb.MovePosition(rb.position + forceDirection * 5f);
                }
                else
                {
                    rb.drag = 1.3f;
                    rb.AddForce(forceDirection * explosionForce, ForceMode.Impulse);
                    StartCoroutine(LimitVelocity(rb));
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
        explosionRadius = baseExplosionRadius;
    }
}
