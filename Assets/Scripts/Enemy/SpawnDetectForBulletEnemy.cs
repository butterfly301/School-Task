using System;
using UnityEngine;

public class SpawnDetectForBulletEnemy : MonoBehaviour
{
    private void OnEnable()
    {
        // 检测周�?0米范围内是否存在挂载了BulletEnemy组件的物�?
        Collider[] colliders = Physics.OverlapSphere(transform.position, 30f);

        foreach (Collider collider in colliders)
        {
            // 排除自身检�?
            if (collider.gameObject == gameObject) continue;
            
            if (collider.gameObject.GetComponent<BulletEnemyAI>() != null || collider.gameObject.CompareTag("LanderPoint"))
            {
                // 如果检测到符合条件的物体，销毁自�?
                MyPooler.ObjectPooler.Instance.ReturnToPool("BulletEnemy",gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Interactive>()||other.gameObject.GetComponent<ExplosiveBarrel>()
                                                        ||other.gameObject.GetComponent<BlockingStripFromEnemy>())
        {
            MyPooler.ObjectPooler.Instance.ReturnToPool("BulletEnemy",gameObject);
        }
    }
}
