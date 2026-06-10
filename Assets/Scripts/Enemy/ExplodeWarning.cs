using System;
using UI;
using UnityEngine;

public class ExplodeWarning : MonoBehaviour
{
    private ExplodeEnemy explodeEnemy;

    private void Start()
    {
        explodeEnemy = GetComponentInParent<ExplodeEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!explodeEnemy.isEnemyBoom&&explodeEnemy.isEnemyAlive)
        {
            FightUIManager.Instance.visionPanel.onWarningEnable(transform);
        }
            
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            FightUIManager.Instance.visionPanel.onWarningKeep(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            FightUIManager.Instance.visionPanel.onWarningDisable();
    }
}
