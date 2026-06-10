using UI;
using UnityEngine;
using UnityEngine.Events;

public class Interactive : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "PlayAnimation";
    protected GameObject player;
    public UnityEvent InteractEvent;
    protected bool canInteract;
    protected PlayerCharacter playerState;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerState = player.GetComponent<PlayerCharacter>();
    }
    
    void Update()
    {
        if(canInteract)
            Interact();
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            FightUIManager.Instance.visionPanel.onInteractTipKeep(transform);
        }
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            canInteract = true;
            FightUIManager.Instance.visionPanel.onInteractTipEnable(transform);
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            canInteract = false;
            FightUIManager.Instance.visionPanel.onInteractTipDisable();
        }
    }
    
    public virtual void Interact()
    {
        // 检查是否按下右键
        if (Input.GetKeyDown(KeyCode.E)&&playerState.isAlive)
            InteractEvent.Invoke();
    }

    public virtual void MakeSomeReaction()
    {
        FightUIManager.Instance.visionPanel.onInteractTipDisable();
        PlayAnimation();
    }
    
    protected void PlayAnimation()
    {
        if (animator != null)
        {
            animator.Play(triggerName);
        }
    }

    /*protected void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }*/
   /*protected virtual bool IsMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider == GetComponent<Collider>();
        }
        return false;
    }*/
   
}
