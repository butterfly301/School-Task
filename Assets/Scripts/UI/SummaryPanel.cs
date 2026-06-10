using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class SummaryPanel : MonoBehaviour
{
    public Animator animator;
    [Header("Text References")]
    public TextMeshProUGUI timeSurvivedText;
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI totalShieldsText;
    public TextMeshProUGUI distanceTraveledText;
    public TextMeshProUGUI survivalStatusText;
    public TextMeshProUGUI totalKillsText;
    public TextMeshProUGUI totalAttacksText;
    public TextMeshProUGUI killsPerMinuteText;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        SetSummaryData();
    }

    public void WhichAnimationShouldBeShown(string result)
    {
        animator.SetTrigger(result);
    }
    
    public void SetSummaryData()
    {
        GameStatsManager stats = GameStatsManager.Instance;

        // 格式化时间（分钟:秒）
        int minutes = Mathf.FloorToInt(stats.timeSurvived / 60f);
        int seconds = Mathf.FloorToInt(stats.timeSurvived % 60f);
        timeSurvivedText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        totalCoinsText.text = stats.totalCoins.ToString();
        totalShieldsText.text = stats.totalShields.ToString();
        distanceTraveledText.text = (stats.distanceTraveled/10).ToString("F1");
        
        // 存活状态
        survivalStatusText.text = stats.isAlive ? "运转中" : "已报废";
        survivalStatusText.color=stats.isAlive ? Color.green : Color.red;

        totalKillsText.text = stats.totalKills.ToString();
        totalAttacksText.text = stats.totalAttacks.ToString();
        
        // 每分钟击杀数（保留1位小数）
        killsPerMinuteText.text = stats.GetKillsPerMinute().ToString("F1");
    }

    public void ResetSummaryData()
    {
        StartCoroutine(ExecuteAfterDelay(5));
    }

    IEnumerator ExecuteAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameStatsManager.Instance.ResetStats();
    }
}
