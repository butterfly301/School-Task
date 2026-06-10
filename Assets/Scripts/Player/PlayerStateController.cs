using System;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public Inventory inventory;
    private Outline outline;
    private PlayerAnimation playerAnimation;
    private PlayerCharacter playerCharacter;
    private PlayerController playerController;
    private CharacterController characterController;
    private DistanceTracker distanceTracker;
    private Animator animator;
    private GameObject modelObject;
    public PlayerAttack playerAttack;

    private void Awake()
    {
        inventory = new Inventory();
        outline = GetComponent<Outline>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerCharacter = GetComponent<PlayerCharacter>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        distanceTracker = GetComponent<DistanceTracker>();
        animator = GetComponentInChildren<Animator>();
        modelObject = animator != null ? animator.gameObject : null;
    }

    public void EnablePlayer()
    {
        if (modelObject != null) modelObject.SetActive(true);
        outline.enabled = true;
        playerAnimation.enabled = true;
        playerCharacter.enabled = true;
        playerController.enabled = true;
        characterController.enabled = true;
        distanceTracker.enabled = true;
        animator.enabled = true;
        playerAttack.enabled = true;
    }

    public void DisablePlayer()
    {
        outline.enabled = false;
        playerAnimation.enabled = false;
        playerCharacter.isAlive = false;
        playerCharacter.enabled = false;
        playerController.enabled = false;
        characterController.enabled = false;
        distanceTracker.enabled = false;
        playerAttack.enabled = false;
        if (modelObject != null) modelObject.SetActive(false);
    }

    public void ResetPlayer()
    {
        inventory.ResetInventory();
        distanceTracker.ResetDistance();
        playerAnimation.TriggerIdle();
        playerCharacter.Set();
    }
}
