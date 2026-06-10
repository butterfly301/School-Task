using System;
using System.Collections;
using UnityEngine;

public class BossAI : EnemyAI
{
    public enum BossSkill
    {
        None,       // 无技能
        Skill1,     // 技能1
        Skill2,     // 技能2
        Skill3      // 技能3
    }

    [System.Serializable]
    public class BossSkillData
    {
        public BossSkill skillType;   // 技能类型
        public float cooldown;        // 冷却时间
        public float castTime;        // 施法时间
        public float range;           // 技能范围
        [HideInInspector] public float lastUsedTime; // 上次使用时间
    }
    
    public BossSkillData[] skills;    // 技能数组
    private BossSkill currentSkill = BossSkill.None; // 当前技能
    private bool isCasting = false;   // 是否正在施法
    
    public GameObject damageArea;
    public GameObject warningLight;
    public GameObject bossLight;
    public GameObject portal;

    public string bossName;
    public string[] ridiculeWords;

    protected override void Start()
    {
        base.Start();
        
        // 初始化技能最后使用时间
        foreach (var skill in skills)
        {
            skill.lastUsedTime = -skill.cooldown; // 开始时技能可用
        }
    }

    protected virtual void OnEnable()
    {
        damageArea.SetActive(true);
        warningLight.SetActive(true);
        bossLight.SetActive(true);
    }

    protected virtual void OnDisable()
    {
        damageArea.SetActive(false);
        warningLight.SetActive(false);
        bossLight.SetActive(false);
        if(portal!=null)
        portal.SetActive(true);
    }

    protected override void Update()
    {
        if (isCasting) return; // 施法时不改变状态或逻辑
        
        base.Update();
    }

    protected override void Attack()
    {
        // Boss特定的攻击逻辑，选择使用哪个技能
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            BossSkill chosenSkill = ChooseSkill();
            if (chosenSkill != BossSkill.None)
            {
                StartCoroutine(CastSkill(chosenSkill));
            }
            else
            {
                // 没有可用技能时使用基础攻击
                base.Attack();
            }
        }
    }

    private BossSkill ChooseSkill()
    {
        // 根据冷却时间和距离检查可用技能
        foreach (var skill in skills)
        {
            if (Time.time - skill.lastUsedTime >= skill.cooldown && 
                distanceToPlayer <= skill.range)
            {
                return skill.skillType;
            }
        }
        return BossSkill.None;
    }

    private IEnumerator CastSkill(BossSkill skill)
    {
        isCasting = true;
        currentSkill = skill;
        navMeshAgent.isStopped = true; // 施法时停止移动
        
        // 查找技能数据
        BossSkillData skillData = null;
        foreach (var s in skills)
        {
            if (s.skillType == skill)
            {
                skillData = s;
                break;
            }
        }

        if (skillData != null)
        {
            // 开始技能施法视觉效果/音效
            OnSkillStart(skill);
            
            // 等待施法时间
            yield return new WaitForSeconds(skillData.castTime);
            
            // 执行技能效果
            ExecuteSkill(skill);
            
            // 更新冷却时间
            skillData.lastUsedTime = Time.time;
            lastAttackTime = Time.time;
            
            // 结束技能效果
            OnSkillEnd(skill);
        }

        currentSkill = BossSkill.None;
        navMeshAgent.isStopped = false;
        isCasting = false;
    }

    protected virtual void ExecuteSkill(BossSkill skill)
    {
        switch (skill)
        {
            case BossSkill.Skill1:
                // 实现技能1逻辑
                Debug.Log("执行技能1");
                break;
                
            case BossSkill.Skill2:
                // 实现技能2逻辑
                Debug.Log("执行技能2");
                break;
                
            case BossSkill.Skill3:
                // 实现技能3逻辑
                Debug.Log("执行技能3");
                break;
        }
    }

    protected virtual void OnSkillStart(BossSkill skill)
    {
        // 播放动画、音效等
        Debug.Log("施放技能" + skill);
    }

    protected virtual void OnSkillEnd(BossSkill skill)
    {
        // 清理技能效果
        
    }
    
    public void ForceStopSkill()
    {
        if (!isCasting) return; // 如果没有在施法，直接返回
    
        // 停止所有协程（包括当前正在施法的协程）
        StopAllCoroutines();
    
        // 重置状态
        isCasting = false;
        currentSkill = BossSkill.None;
        navMeshAgent.isStopped = false;
    
        // 调用技能结束的逻辑
        OnSkillEnd(currentSkill);
    }
}