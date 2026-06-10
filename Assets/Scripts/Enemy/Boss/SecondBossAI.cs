using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SecondBossAI : BossAI
{
    [Header("视觉效果")] 
    public GameObject smokeEffect;
    
    [Header("组件引用")]
    public Animator animator;
    public FacePlayer facePlayer;
    
    [Header("Skill 1: Summon Minions")]
    public Transform[] summonPositions;      // 四个召唤位置

    [Header("Skill 2: Missile & Vortex")]
    public Transform[] firePoint;
    public int missileCount = 2;             // 导弹数量
    public float missileFireInterval = 1f; // 导弹发射间隔

    /*protected override void Start()
    {
        base.Start();
        
        // 初始化Boss技能数据
        skills = new BossSkillData[2];
        
        // 技能1数据
        skills[0] = new BossSkillData()
        {
            skillType = BossSkill.Skill1,
            cooldown = 5f,
            castTime = 1f,  // 1秒吟唱时间
            range = 100f
        };
        
        // 技能2数据
        skills[1] = new BossSkillData()
        {
            skillType = BossSkill.Skill2,
            cooldown = 5f,
            castTime = 0.5f,
            range = 100f
        };
    }*/

    protected override void OnEnable()
    {
        base.OnEnable();
        animator.enabled = true;
        facePlayer.enabled = true;
        // 改为延迟调用，避免初始化未完成
        StartCoroutine(DelayedMissileLaunch());
        SaySomethingJunkWord();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        animator.enabled = false;
        facePlayer.enabled = false;
    }
    
    private IEnumerator DelayedMissileLaunch()
    {
        yield return null; // 等待一帧
        StartCoroutine(ServiltyShown());
    }

    protected override void ExecuteSkill(BossSkill skill)
    {
        switch (skill)
        {
            case BossSkill.Skill1:
                StartCoroutine(Skill1_SummonMinions());
                break;
                
            case BossSkill.Skill2:
                StartCoroutine(Skill2_MissileAndVortex());
                break;
        }
    }

    private IEnumerator Skill1_SummonMinions()
    {
        // 1秒吟唱时间已在基类中通过castTime处理
        
        // 在四个位置召唤魔物
        for (int i = 0; i < Mathf.Min(4, summonPositions.Length); i++)
        {
            SummonMinion(summonPositions[i].position);
        }
        
        yield return null;
    }

    private void SummonMinion(Vector3 position)
    {
        GameObject minion= MyPooler.ObjectPooler.Instance.GetFromPool("Minion", position, Quaternion.identity);
        // 确保有NavMeshAgent组件
        var agent = minion.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = minion.AddComponent<NavMeshAgent>();
            // 设置敌人速度
            agent.speed = minion.GetComponent<EnemyAI>().speed;
        }
    }

    private IEnumerator Skill2_MissileAndVortex()
    {
        // 发射跟踪导弹
        for (int i = 0; i < missileCount; i++)
        {
            LaunchHomingMissile();
            yield return new WaitForSeconds(missileFireInterval);
        }
        
        // 创建漩涡
        //CreateVortex();
    }

    private void LaunchHomingMissile()
    {
        // 检查关键变量是否为空
        if (player == null || firePoint == null || firePoint.Length < 2)
        {
            Debug.LogWarning("玩家或发射点未初始化！");
            return;
        }
        Vector3 direction = player.position-transform.position;
        // 计算旋转使物体Z轴指向玩家
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        MyPooler.ObjectPooler.Instance.GetFromPool("TrackBullet", firePoint[0].position, targetRotation);
        MyPooler.ObjectPooler.Instance.GetFromPool("TrackBullet", firePoint[1].position, targetRotation);
        
    }

    private IEnumerator ServiltyShown()
    {
        for (int i = 0; i < 6; i++)
        {
            Vector3 direction = player.position-transform.position;
            // 计算旋转使物体Z轴指向玩家
            Quaternion targetRotation = Quaternion.LookRotation(direction);
        
            MyPooler.ObjectPooler.Instance.GetFromPool("TrackBulletFast", firePoint[2].position, targetRotation);
            MyPooler.ObjectPooler.Instance.GetFromPool("TrackBulletFast", firePoint[3].position, targetRotation);
            MyPooler.ObjectPooler.Instance.GetFromPool("TrackBulletFast", firePoint[4].position, targetRotation);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void CreateVortex()
    {
        if (player == null) return;
        
        // 在玩家当前位置创建漩涡
        GameObject vortex= MyPooler.ObjectPooler.Instance.GetFromPool("Vortex",transform.position, Quaternion.identity);
        var agent = vortex.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            agent = vortex.AddComponent<NavMeshAgent>();
            // 设置敌人速度
            agent.speed = vortex.GetComponent<Whirlpool>().speed;
        }
    }

    protected override void OnSkillStart(BossSkill skill)
    {
        MinionEnemy[] minions = FindObjectsOfType<MinionEnemy>();
        if (minions.Length == 0)
        {
            animator.SetTrigger("Attack");
            EnableSmokeEffect();
            StartCoroutine(DisableSmokeEffect());
        }
        else
        {
            FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll(bossName+"："+"在你消灭我的傀儡之前");
            FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll(bossName+"："+"我不会再打开舱门");
        } 

        if(Random.value < 0.5f)//放技能时有50%概率飙垃圾话
            SaySomethingJunkWord();
    }

    void EnableSmokeEffect()
    {
        if (!smokeEffect.activeSelf)
        {
            smokeEffect.SetActive(true);
        }
    }

    IEnumerator DisableSmokeEffect()
    {
        yield return new WaitForSeconds(1.5f);
        smokeEffect.SetActive(false);
    }

    protected override void OnSkillEnd(BossSkill skill)
    {
        animator.SetTrigger("AttackEnd");
    }

    void SaySomethingJunkWord()
    {
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll(bossName+"："+ridiculeWords[Random.Range(0, ridiculeWords.Length)]);
    }
}