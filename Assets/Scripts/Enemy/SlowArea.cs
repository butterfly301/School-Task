using System;
using UI;
using UnityEngine;
using UnityEngine.Events;

public class SlowArea : MonoBehaviour
{
    private PlayerController playerController;
    private float playerOriginalSpeed=20f;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerOriginalSpeed = playerController.originalMoveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.moveSpeed =7f;
            BlockTheSignal();
            glitchDamageEnable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ReturnPlayerOriginalSpeed();
            RecoverTheSignal();
            glitchDamageDisable();
        }
    }

    public void ReturnPlayerOriginalSpeed()
    {
        playerController.moveSpeed = playerOriginalSpeed;
    }

    public void BlockTheSignal()
    {
        FightUIManager.Instance.signalController.BlockTheSignal();
    }

    public void RecoverTheSignal()
    {
        FightUIManager.Instance.signalController.RecoverTheSignal();
    }

    public void glitchDamageEnable()
    {
        FightUIManager.Instance.visionPanel.onGlitchDamageEnable();
    }

    public void glitchDamageDisable()
    {
        FightUIManager.Instance.visionPanel.onGlitchDamageDisable();
    }
}
