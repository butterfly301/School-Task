using System;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Portal : Interactive
{
    public UnityEvent onTeleport;
    public GameObject portalGlow;

    private void OnEnable()
    {
        Instantiate(portalGlow,transform.position,Quaternion.identity);
    }

    public override void MakeSomeReaction()
    {
        base.MakeSomeReaction();
        DisablePlayer();
        TimeRemainingTransToMoney();
        MyPooler.ObjectPooler.Instance.ResetAllPools();
        onTeleport.Invoke();
    }
    private void DisablePlayer()
    { 
        player.GetComponent<PlayerStateController>().DisablePlayer();
        FightUIManager.Instance.StopCountdownTimer();
    }

    public void TimeRemainingTransToMoney()
    {
        var playerCharacter = player.GetComponent<PlayerCharacter>();
        playerCharacter.money+= (int)FightUIManager.Instance.countdownTimer.GetRemainingTime();
    }
}