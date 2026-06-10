using System;
using UnityEngine;

public class KillEveryEnemy : MonoBehaviour
{
    public float targetRadius = 1f; // 目标半径
    public float duration = 1f; // 变化持续时间

    private SphereCollider sphereCollider;
    private float startTime;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>(); // 获取 SphereCollider 组件
        sphereCollider.radius = 0.1f; // 初始半径为 0.1
        startTime = Time.time; // 记录开始时间
    }

    void Update()
    {
        float elapsedTime = Time.time - startTime; // 已过去的时间
        if (elapsedTime < duration)
        {
            // 使用 Mathf.Lerp 在 2 秒内从 0 变到目标半径
            sphereCollider.radius = Mathf.Lerp(0f, targetRadius, elapsedTime / duration);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Enemy>();
        if(enemy!=null)
            enemy.OnDie.Invoke();
    }
}