using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    // 单例模式
    public static GameStatsManager Instance { get; private set; }

    // 统计数据
    public float timeSurvived;          // 存活时间（秒）
    public int totalCoins;              // 总金币
    public int totalShields;            // 总护盾
    public float distanceTraveled;      // 行走距离（米）
    public bool isAlive;                // 是否存活
    public int totalKills;              // 总击杀数
    public int totalAttacks;           // 总攻击次数

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 更新存活时间（在Update中调用）
    public void UpdateTimeSurvived()
    {
        timeSurvived += Time.deltaTime;
    }

    // 计算每分钟击杀数
    public float GetKillsPerMinute()
    {
        if (timeSurvived <= 0) return 0;
        return totalKills / (timeSurvived / 60f);
    }

    // 重置所有统计数据（开始新游戏时调用）
    public void ResetStats()
    {
        timeSurvived = 0;
        totalCoins = 0;
        totalShields = 0;
        distanceTraveled = 0;
        isAlive = true;
        totalKills = 0;
        totalAttacks = 0;
    }
}