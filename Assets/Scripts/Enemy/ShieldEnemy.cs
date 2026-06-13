using System;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShieldEnemy : Enemy
{
    private bool hasShield;
    [SerializeField] AudioEventChannel channels;
    public GameObject shieldModel;

    protected override void OnEnable()
    {
        base.OnEnable();
        hasShield = true;
    }

    public override void TakeDamage()
    {
        if (hasShield)
        {
            hasShield = false;
            FightUIManager.Instance.visionPanel.onShieldBreakEnable(transform);
            shieldModel.SetActive(false);
            channels.Raise3D(SoundEvent.ShieldBreak, transform.position);
        }
        else
        {
            base.TakeDamage();
        }
    }
    
    public override void OnPlayerDeath()
    {
        base.OnPlayerDeath();
       
        Set();
    }

    public override void Set()
    {
        base.Set();
        hasShield = true;
        shieldModel.SetActive(true);
    }

}
