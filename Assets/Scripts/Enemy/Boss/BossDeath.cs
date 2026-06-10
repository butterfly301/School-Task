using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class BossDeath : MonoBehaviour
{
    [Header("组件引用")] 
    private BossAI bossAI;
    private Guide guide;
    [Header("物体引用")] public GameObject explodeEffect;

    private void Start()
    {
        bossAI = GetComponent<BossAI>();
        guide = GetComponent<Guide>();
    }

    public void BossDie()
    {
        bossAI.enabled = false;
        FightUIManager.Instance.StopCountdownTimer();
        guide.StartShowGuide();
        StartCoroutine(ReturnNormalTimeScale());
    }

    IEnumerator ReturnNormalTimeScale()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        Instantiate(explodeEffect, transform.position, Quaternion.identity);
        Destroy(bossAI.gameObject);
    }
}
