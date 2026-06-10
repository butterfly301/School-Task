using UnityEngine;

public class DistanceTracker : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Only count distance when these keys are pressed")]
    private KeyCode[] movementKeys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    
    private Vector3 lastPosition;
    private bool isTracking = true;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (!isTracking || !IsMovingByInput()) return;
        
        // 使用物理引擎的位置变化计算距离（更精确）
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        GameStatsManager.Instance.distanceTraveled += distanceMoved;
        lastPosition = transform.position;
    }

    // 检测是否正在通过WSAD移动
    private bool IsMovingByInput()
    {
        foreach (KeyCode key in movementKeys)
        {
            if (Input.GetKey(key))
            {
                return true;
            }
        }
        return false;
    }

    // 外部控制方法
    public void StartTracking() => isTracking = true;
    public void StopTracking() => isTracking = false;
    public void ResetDistance()
    {
        GameStatsManager.Instance.distanceTraveled = 0;
        lastPosition = transform.position;
    }
}