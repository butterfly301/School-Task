using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/ShieldEventSO")]
public class ShieldEventSO : ScriptableObject
{
    public UnityAction<PlayerCharacter> OnEventRaised;

    public void RaiseEvent(PlayerCharacter playerCharacter)
    {
        OnEventRaised?.Invoke(playerCharacter);
    }
}
