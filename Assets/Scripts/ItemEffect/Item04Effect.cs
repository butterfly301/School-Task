using UnityEngine;
[CreateAssetMenu(menuName = "ItemEffect/Item04Effect")]
public class Item04Effect : ItemEffect
{
    public static event System.Action OnItem04Effect; // 定义事件
    public override void ApplyEffect()
    {
        OnItem04Effect?.Invoke();
    }
}