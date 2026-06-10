using System;
using UnityEngine;
public class BulletEnemyAI : EnemyAI
{
    public Transform firePoint;
    public string bulletTag;
    private Vector3 direction;
    private Quaternion targetRotation;
    [SerializeField] private FacePlayer facePlayer;
    [SerializeField] private AudioEventChannel channel;

    private void OnEnable()
    {
        ToggleFacePlayer();
    }

    protected override void Update()
    {
        base.Update();
        direction = player.position-transform.position;
        // 计算旋转使物体Z轴指向玩�?
        targetRotation = Quaternion.LookRotation(direction);
    }

    protected override void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            MyPooler.ObjectPooler.Instance.GetFromPool(bulletTag, firePoint.position, targetRotation);
            lastAttackTime = Time.time;
           channel.Raise3D(SoundEvent.MissileFire, transform.position);//导弹发射音效
            //lastAttackTime = Time.time;
        }
    }

    public void ToggleFacePlayer()
    {
        facePlayer.enabled = !facePlayer.enabled;
    }
    
}
