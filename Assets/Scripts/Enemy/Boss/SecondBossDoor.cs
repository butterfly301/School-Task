using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

public class SecondBossDoor : MonoBehaviour
{
    [Header("数值")] 
    private bool isDead;
    [Header("组件引用")]
    public Boss secondBoss;
    [Header("物体引用")] 
    public GameObject sparkEffect;
    public GameObject door;
    [Header("事件")] 
    public UnityEvent OnHurt;

    private void OnEnable()
    {
        if (isDead)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void TakeDamage()
    {
        isDead = true;
        door.SetActive(false);
        sparkEffect.SetActive(true);
        secondBoss.TakeDamage();
        /*foreach (var door in otherDoor)
        {
            door.SetActive(false);
        }*/
        this.gameObject.SetActive(false);
    }
}
