using UnityEngine;
using DG.Tweening;

public class PitDisappear : MonoBehaviour
{
    private float delayBeforeFade = 1f; // 等待时间(�?
    private float fadeDuration = 1f;    // 淡出持续时间(�?
    
    private void Awake()
    {
        DOTween.defaultAutoKill = true;
    }

    void Start()
    {
        // 获取SpriteRenderer组件
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // 设置初始透明度为1(完全不透明)
        Color originalColor = spriteRenderer.color;
        originalColor.a = 0.67f;
        spriteRenderer.color = originalColor;
        
        // 延迟后开始淡�?
        DOVirtual.DelayedCall(delayBeforeFade, () => {
            // 使用DOTween渐变透明度到0
            spriteRenderer.DOFade(0f, fadeDuration)
                .OnComplete(() => {
                    // 透明度为0时销毁物�?
                    Destroy(gameObject);
                });
        });
    }
}
