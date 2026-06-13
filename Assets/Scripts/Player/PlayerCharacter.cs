using System;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour
{
    [Header("事件")]
    public UnityEvent OnHurt;
    public UnityEvent OnDeath;
    [Header("属性")]
    public int money;
    public int shield;
    private bool canRevive;
    [HideInInspector] public bool isAlive;
    public string[] dieWords;
    [Header("组件引用")]
    private PlayerAnimation playerAnimation;
    private PlayerController playerController;
    [SerializeField] private AudioEventChannel channel;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void OnEnable()
    {
        isAlive = true;
        canRevive = SaveManager.Instance != null && SaveManager.Instance.GetSavedCanRevive();
        Item06Effect.OnItem06Effect += EnableRevive;
        Item05Effect.OnItem05Effect += AddShield;
    }

    private void OnDisable()
    {
        Item06Effect.OnItem06Effect -= EnableRevive;
        Item05Effect.OnItem05Effect -= AddShield;
    }

    public void Set()
    {
        isAlive = false;
        money = 0;
        shield = 1;
        canRevive = false;
    }

    public bool CanRevive
    {
        get => canRevive;
        set => canRevive = value;
    }

    public void TakeDamage()
    {
        if(playerController.isInvulnerable||!isAlive)
            return;
        if (shield > 0)
        {
            shield--;
            FightUIManager.Instance.visionPanel.onShieldBreakEnable(transform);
        }
        else
        {
            OnDeath.Invoke();
        }
    }

    private void EnableRevive()
    {
        canRevive = true;
    }

    private void AddShield()
    {
        shield++;
        GameStatsManager.Instance.totalShields += 1;
    }
    
    public void PlayerDeath()
    {
        PlayerDeathBroadcaster.Broadcast();
        channel.Raise2D(SoundEvent.Hurt);
        isAlive = false;
        playerController.enabled = false;
        GameStatsManager.Instance.isAlive = false;
        StartCoroutine(SetVolun());
        GameFlowCoordinator.Instance.EnterFlow(GameFlowState.Gameplay);
        FightUIManager.Instance.StopCountdownTimer();
        FightUIManager.Instance.visionPanel.onGlitchDamageEnable();
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR："+dieWords[Random.Range(0,dieWords.Length)]);
        StartCoroutine(ExecuteAfterDelay(3));
    }

    IEnumerator ExecuteAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        FightUIManager.Instance.visionPanel.onGlitchDamageDisable();
        if (canRevive)
        {
            Revive();
        }
        else
        {
            FixedUIManager.Instance.ShowSummaryPanel("Failure");
        }
    }
    
    public GameObject shockWave;
    
    Vector3 offset=new Vector3(0f,0.5f,0f);
    void Revive()
    {
        Instantiate(shockWave, transform.position+offset, Quaternion.identity);
        FightUIManager.Instance.StartCountdownTimer();
        playerAnimation.TriggerRevive();
        money = 0;
        playerController.enabled = true;
        isAlive = true;
        GameStatsManager.Instance.isAlive = true;
        canRevive = false;
    }
    
    IEnumerator SetVolun()
    {
        
        yield return new WaitForSeconds(1.3f);
        FindObjectOfType<SoundManager>().SetSFXVolun();

    }
    
}
