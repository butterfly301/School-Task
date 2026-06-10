using System.Collections;
using UI;
using UnityEngine;

public class AfterLand : StateMachineBehaviour
{
    private float explosionRadius = 5f;
    [SerializeField] private GameObject boomEffect;
    [SerializeField] private AudioEventChannel audioEventChannel;
    
    private Transform player;
    public VoidEventSO OnLand;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioEventChannel.Raise2D(SoundEvent.AfterLand);
        Transform transform= animator.gameObject.GetComponent<Transform>();
        player=GameObject.FindGameObjectWithTag("Player").transform;
        FightUIManager.Instance.visionPanel.onInteractTipEnable(transform);
        OnLand.OnEventRaised?.Invoke();
        player.position = animator.transform.position;
        player.GetComponent<PlayerStateController>().DisablePlayer();
    }
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FightUIManager.Instance.visionPanel.onInteractTipKeep(animator.transform);
        if (Input.GetKeyDown(KeyCode.E))
        {
            DestoryLander(animator);
            EnablePlayer();
            Collider(animator);
        }
    }
    
    public void DestoryLander(Animator animator)
    {
        FightUIManager.Instance.visionPanel.onInteractTipDisable();
        audioEventChannel.Raise3D(SoundEvent.Explosion,animator.transform.position);
        Destroy(animator.gameObject);
    }
    
    private void EnablePlayer()
    {
        player.GetComponent<PlayerStateController>().EnablePlayer();
        FightUIManager.Instance.StartCountdownTimer();
    }
    
    private void Collider(Animator animator)
    {
        GameObject boom=Instantiate(boomEffect, animator.transform.position, Quaternion.identity);
        boom.transform.localScale=new Vector3(3,3,3);
        Collider[] colliders = Physics.OverlapSphere(animator.transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            Vector3 direction =  hit.transform.position-animator.transform.position;
            if (rb != null )
            {   
                if(hit.gameObject.layer != LayerMask.NameToLayer("CanGo"))
                {
                    rb.velocity = direction.normalized * rb.mass;
                }
                for(int i=0;i<2;i++)
                    hit.GetComponent<Enemy>()?.OnHurt?.Invoke();
            }

            /*if (rb != null&&hit.gameObject.layer!=LayerMask.NameToLayer("CanGo") )
            {
                Vector3 forceDirection = (rb.transform.position - transform.position).normalized;
                if (rb.isKinematic)
                {

                    rb.MovePosition(rb.position + forceDirection * 5f); // ��΢����
                }
                else
                {

                    rb.AddForce(forceDirection * explosionForce, ForceMode.Impulse);
                    StartCoroutine(LimitVelocity(rb)); // ��������ٶ�?
                }
                hit.GetComponent<PlayerCharacter>()?.OnHurt.Invoke();
                hit.GetComponent<Enemy>()?.OnHurt.Invoke();
            }*/
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
