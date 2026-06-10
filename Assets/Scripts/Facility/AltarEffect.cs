using UnityEngine;

[CreateAssetMenu(menuName = "FacilityEffect/AltarEffect")]
public class AltarEffect : ScriptableObject
{
    public static event System.Action OnAltarEffect; // 定义事件

    public void ApplyEffect()
    {
        OnAltarEffect?.Invoke();
    }
}