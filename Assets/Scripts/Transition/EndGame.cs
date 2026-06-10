using System;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public bool fadeScreen = true;
    public PlayerStateController playerStateController;

    private void Start()
    {
        playerStateController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateController>();
    }

    public void TriggerAction()
    {
        MyPooler.ObjectPooler.Instance.ResetAllPools();
        GameStatsManager.Instance.ResetStats();
        playerStateController.ResetPlayer();
        playerStateController.DisablePlayer();
        loadEventSO.RaiseLoadEvent(sceneToGo, positionToGo, fadeScreen);
    }
    
}
