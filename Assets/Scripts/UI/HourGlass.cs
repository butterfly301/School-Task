using System;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class HourGlass : MonoBehaviour
{
    public float duration = 1f; // 动画持续时间
    private RectTransform rectTransform;
    
    private void Awake()
    {
        DOTween.defaultAutoKill = true;
    }
    void Start()
    {
        // 获取RectTransform组件
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Item08Effect.OnItem08Effect += ReverseHourGlass;
    }

    private void OnDisable()
    {
        Item08Effect.OnItem08Effect -= ReverseHourGlass;
    }

    private void ReverseHourGlass()
    {
        // 沿Z轴旋�?80度（UI元素应该使用DOLocalRotate�?
        rectTransform.DOLocalRotate(new Vector3(0, 0, 180), duration)
            .SetEase(Ease.OutQuad); // 添加缓动效果
    }
}
