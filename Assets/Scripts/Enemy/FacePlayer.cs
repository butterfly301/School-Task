using System;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player != null)
        {
            // 计算方向向量
            Vector3 direction = player.position - transform.position;
            
            // 忽略Y轴差异，只考虑水平方向
            direction.y = 0;
            
            // 只有当方向不是零向量时才旋转
            if (direction != Vector3.zero)
            {
                // 使用Quaternion.LookRotation让Z轴指向Player的水平方�?
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                // 只保留Y轴旋转，其他轴保持原�?
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,  // 保持原有X旋转
                    targetRotation.eulerAngles.y,     // 只应用新的Y旋转
                    transform.rotation.eulerAngles.z   // 保持原有Z旋转
                );
            }
        }
    }
}
