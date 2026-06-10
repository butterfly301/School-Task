using UnityEngine;

public class PingPongValue : MonoBehaviour
{
    private int minValue = 0;
    private int maxValue = 1;
    private float speed = 1f;
    
    public Light light;

    void Update()
    {
        // 使用Mathf.PingPong在minValue和maxValue之间来回变化
        light.intensity = Mathf.PingPong(Time.time * speed, maxValue - minValue) + minValue;
    }
}