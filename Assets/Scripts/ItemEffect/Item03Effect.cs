using UnityEngine;
[CreateAssetMenu(menuName = "ItemEffect/Item03Effect")]
public class Item03Effect : ItemEffect
{
    public float explosionRadius;
    public static event System.Action OnItem03Effect; // 定义事件
    public override void ApplyEffect()
    {
        OnItem03Effect?.Invoke();
    }
    
}