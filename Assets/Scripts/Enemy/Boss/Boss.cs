using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : Enemy
{
    [Header("数值")]
    public int health;
    
    [Header("组件引用")]
    public BossAI bossAI;
    
    public string bossName;
    public string[] hurtWords;
    
    private void Start()
    {
        if(enemyAI==null)
            enemyAI = GetComponentInParent<EnemyAI>();
        bossAI = GetComponentInParent<BossAI>();
    }

    protected override void OnEnable()
    {
        maxHealth = Mathf.Max(1, health);
        base.OnEnable();
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            OnDie.Invoke();
    }*/

    public override void TakeDamage()
    {
        SayHurtWord();
        bossAI.ForceStopSkill();
        if (ApplyDamage(1))
            OnDie.Invoke();
    }

    void SayHurtWord()
    {
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll(bossName+"："+hurtWords[Random.Range(0, hurtWords.Length)]);
    }
}
