using System;
using UnityEngine;
using UI;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public bool fadeScreen = true;
    [SerializeField] private AudioEventChannel channel;

    public void TriggerAction()
    {
        loadEventSO.RaiseLoadEvent(sceneToGo, positionToGo, fadeScreen);
    }
    public void audios()
    {
        channel.Raise2D(SoundEvent.PressButton);
    }
    
    public void DealSuccess()
    {
        FixedUIManager.Instance.ShowSummaryPanel("Success");
    }
}
