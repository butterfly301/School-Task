using System;
using TMPro;
using UnityEngine;

public class SignalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform teleporter;    // 拖入传送门对象
    [SerializeField] Transform player;       // 拖入玩家对象
    [SerializeField] Animator signalAnimator; // 拖入UI Animator组件
    public GameObject blockTheSignal;
    public TextMeshProUGUI text;

    [Header("Distance Settings")]
    [SerializeField] float level1Max = 200f;
    [SerializeField] float level2Max = 100f;
    [SerializeField] float level3Max = 50f;
    [SerializeField] float fullThreshold = 25f;
    [SerializeField] float hysteresis = 1f;  // 防抖动缓冲值

    private int currentLevel;
    private float prevDistance;
    private float distance;
    
    /*public static SignalController Instance;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }*/

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        teleporter=GameObject.FindGameObjectWithTag("Teleporter").transform;
        
        // 添加距离变化检测降低运算频率
        if(Mathf.Abs(distance - prevDistance) > 0.1f)
        {
            UpdateSignalState(distance);
            prevDistance = distance;
        }
    }

    private void OnEnable()
    {
        if(player!=null&&teleporter!=null)
            distance = Vector3.Distance(player.position, teleporter.position);
        currentLevel = CalculateSignalLevel(distance);
        signalAnimator.SetInteger("SignalLevel", currentLevel);
        signalAnimator.SetFloat("Distance", distance);
    }

    void Update()
    {
        distance = Vector3.Distance(player.position, teleporter.position);
        
        // 添加距离变化检测降低运算频率
        if(Mathf.Abs(distance - prevDistance) > 0.1f)
        {
            UpdateSignalState(distance);
            prevDistance = distance;
        }
    }

    void UpdateSignalState(float distance)
    {
        int newLevel = CalculateSignalLevel(distance);

        if(ShouldChangeLevel(newLevel))
        {
            TriggerLevelChange(newLevel);
            currentLevel = newLevel;
        }

        signalAnimator.SetFloat("Distance", distance);
    }

    int CalculateSignalLevel(float distance)
    {
        if (distance > level1Max)
        {
            return 0;
        } 
        else if (distance > level2Max)
        {
            return 1;
        }
        else if(distance > level3Max)
        {
            return 2;
        }
        else if(distance > fullThreshold) 
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    bool ShouldChangeLevel(int newLevel)
    {
        return Mathf.Abs(newLevel - currentLevel) >= GetRequiredLevelChange();
    }

    int GetRequiredLevelChange()
    {
        // 根据当前等级动态调整敏感度
        return currentLevel switch
        {
            0 => 1,    // 从无信号到一级需要完整变化
            4 => 1,    // 满格到三级需要完整变化
            _ => Mathf.CeilToInt(hysteresis / 15f) // 动态计算
        };
    }

    void TriggerLevelChange(int newLevel)
    {
        //signalAnimator.ResetTrigger("LevelUp");
        //signalAnimator.ResetTrigger("LevelDown");

        if(newLevel > currentLevel)
        {
            signalAnimator.SetTrigger("LevelUp");
            signalAnimator.SetInteger("SignalLevel", newLevel);
            UpdateTip(newLevel);
        }
        else
        {
            signalAnimator.SetTrigger("LevelDown");
            signalAnimator.SetInteger("SignalLevel", newLevel);
            UpdateTip(newLevel);
        }
    }

    private void UpdateTip(int newLevel)
    {
        switch (newLevel)
        {
            case 0:
                text.text = "正在搜寻信号源";
                break;
            case 1:
                text.text = "信号强度：弱";
                break;
            case 2:
                text.text = "信号强度：中"; 
                break;
            case 3:
                text.text = "信号强度：强"; 
                break;
            case 4:
                text.text ="发现信号源";
                break;
        }
    }

    public void BlockTheSignal()
    {
        blockTheSignal.SetActive(true);
        text.text = "信号受到干扰";
    }

    public void RecoverTheSignal()
    {
        blockTheSignal.SetActive(false);
        UpdateTip(CalculateSignalLevel(distance));
    }
}