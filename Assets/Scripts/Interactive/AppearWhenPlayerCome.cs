using System;
using UnityEngine;
using UI;

public class AppearWhenPlayerCome : MonoBehaviour
{
    private bool isTiped;
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private AudioEventChannel AudioEventChannel;

    private void Start()
    {
        isTiped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioEventChannel.Raise2D(SoundEvent.PortalActive);
            if (!isTiped)
            {
                FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：找到传送门！");
                isTiped = true;
            }
            
            FightUIManager.Instance.visionPanel.onInteractTipEnable(transform);
        }

        if ((collisionLayer.value & (1 << other.gameObject.layer)) != 0||other.GetComponent<Shop>()!=null||
            other.GetComponent<Chest>()!=null||other.gameObject.GetComponent<Altar>()!=null)
        {
                other.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FightUIManager.Instance.visionPanel.onInteractTipKeep(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            FightUIManager.Instance.visionPanel.onInteractTipDisable();
        }
    }
}
