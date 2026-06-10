using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlowEnemy : Enemy
{
    public GameObject slowArea;

    protected override void OnEnable()
    {
        base.OnEnable();
        slowArea.SetActive(true);
    }

    public void DisableSlowArea()
    {
        slowArea.SetActive(false);
    }
    public override void OnPlayerDeath()
    {
        base.OnPlayerDeath();
        // ֹͣ��Ϊ�߼�
       Set();
    }
}
