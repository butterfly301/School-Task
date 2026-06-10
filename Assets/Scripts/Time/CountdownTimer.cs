using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UI;

public class CountdownTimer : MonoBehaviour
{
    [Header("组件引用")]
    private TextMeshProUGUI CountdownText;
    public Image[] hourGlassImage;
    public RectTransform hourGlassRect;
    private PlayerCharacter character;
    public Image hourglassUpImage;
    public Image hourglassDownImage;
    [Header("数值")]
    public float initialTime = 60f; // 初始倒计时时间（秒）
    public float timeRemaining; // 剩余时间
    private bool timerIsRunning; // 计时器是否在运行
    public bool isTimerStarted;
    private bool isTiped;
    private bool isPlayed=false;
    [SerializeField] private AudioEventChannel channel;
    
    private void Awake()
    {
        DOTween.defaultAutoKill = true;
    }

    private void Start()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        CountdownText = GetComponent<TextMeshProUGUI>();
        timeRemaining = initialTime;
        isTimerStarted = false;
        isTiped = false;
        //StartTimer(); // 启动计时器
    }

    private void OnEnable()
    {
        Item07Effect.OnItem07Effect += AddTime;
        Item08Effect.OnItem08Effect += ReverseTime;
        AltarEffect.OnAltarEffect += OnDecreaseTimeRemaining;
    }

    private void OnDisable()
    {
        Item07Effect.OnItem07Effect -= AddTime;
        Item08Effect.OnItem08Effect -= ReverseTime;
        AltarEffect.OnAltarEffect -= OnDecreaseTimeRemaining;
    }



    private void Update()
    {
        if (isTimerStarted)
            UpdateTimerDisplay();
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // 减少剩余时间
                GameStatsManager.Instance.UpdateTimeSurvived();
                StopAudio();

            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                character.OnDeath.Invoke();
            }
        }
    }

    // 启动计时器
    public void StartTimer()
    {
        timerIsRunning = true;
    }

    public void StopTimer()
    {
        timerIsRunning = false;
    }

    // 获取剩余时间
    public float GetRemainingTime()
    {
        return timeRemaining;
    }

    private void UpdateTimerDisplay()
    {
        //数字显示
        int seconds = Mathf.FloorToInt(timeRemaining); // 计算秒
        int milliseconds = Mathf.FloorToInt((timeRemaining - seconds) * 100); // 计算毫秒
        if (timeRemaining <= 0)
        {
            seconds = 0;
            milliseconds = 0;
        }

        // 格式化为两位数显示
        CountdownText.text = string.Format("{0:00}:{1:00}", seconds, milliseconds);

        if (timeRemaining <= 10)
        {
            CountDownAudio();
            if (!isTiped)
            {
                StartCoroutine(TimeRunningOutTip());
                isTiped = true;
            }

            CountdownText.color = Color.red;
            for (int i = 0; i < hourGlassImage.Length; i++)
            {
                hourGlassImage[i].color = Color.red;
            }
        }
        else
        {
            CountdownText.color = Color.white;
            for (int i = 0; i < hourGlassImage.Length; i++)
            {
                hourGlassImage[i].color = Color.white;
            }
        }
        //图片显示
        var percentage = Math.Max(Math.Min(timeRemaining / 60, 1), 0);
        hourglassUpImage.fillAmount = percentage;
        hourglassDownImage.fillAmount = 1 - percentage;
    }

    public void ToggleTimer()
    {
        isTimerStarted = !isTimerStarted;
    }

    // 增加时间
    private void AddTime(float secondsToAdd)
    {
        timeRemaining += secondsToAdd;
    }

    //倒转沙漏
    private void ReverseTime()
    {
        timeRemaining = initialTime - timeRemaining;
        hourGlassRect.DOLocalRotate(new Vector3(0, 0, 180), 1f)
            .OnComplete(() =>
            {
                hourGlassRect.localRotation = Quaternion.Euler(0, 0, 0);
            });
    }

    public void OnDecreaseTimeRemaining()
    {
        timeRemaining -= 5;
    }

    IEnumerator TimeRunningOutTip()
    {
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：时间快耗尽了");
        yield return new WaitForSeconds(1f);
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：抓紧时间进入下一关吧");
    }
    public void CountDownAudio()
    {
        if (timeRemaining <= 10)
        {
            channel.Raise2D(SoundEvent.CountDown);
            isPlayed = true;
        }

    }
    public void StopAudio()
    {
        if (timeRemaining > 10&&isPlayed)
        {
            FindObjectOfType<SoundManager>().StopSound(SoundEvent.CountDown);
            isPlayed = false;
        }
    }
}
