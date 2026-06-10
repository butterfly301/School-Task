using System;
using System.Collections;
using UnityEngine;

public class AlwaysRotate : MonoBehaviour
{
    private float speed = 60f; // 旋转速度（度/秒）
    private bool canRotate;
    public float delay;

    private void Start()
    {
        StartCoroutine(EnableRotate());
    }

    IEnumerator EnableRotate()
    {
        yield return new WaitForSeconds(delay);
        canRotate = true;
    }

    void Update()
    {
        // 绕Z轴旋转（Vector3.forward 代表Z轴）
        if(canRotate)
        transform.Rotate(Vector3.forward * speed * Time.deltaTime);
    }
}
