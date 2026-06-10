using UnityEngine;
using DG.Tweening;

public class BlockingStripFromEnemy : MonoBehaviour
{
    public float targetScaleX = 1.0f;
    public float duration = 1.0f;
    public LayerMask layerMask;
    
    private void Awake()
    {
        DOTween.defaultAutoKill = true;
    }
    private void Start()
    {
        // 生成一�?�?60度之间的随机角度
        float randomAngle = Random.Range(0f, 360f);
        // 设置物体的旋转，使其沿Y轴旋转随机角�?
        transform.rotation = Quaternion.Euler(0, randomAngle, 0);
        Vector3 targetScale = new Vector3(targetScaleX, transform.localScale.y, transform.localScale.z);
        transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.gameObject.GetComponent<ItemEffectOnEnemy>()?.Strip();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if ((layerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.gameObject.GetComponent<ItemEffectOnEnemy>()?.StopStrip();
        }
    }
}
