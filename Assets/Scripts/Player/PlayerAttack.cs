using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("引用")]
    public PlayerController playercontroller;
    public AudioEventChannel channel;
    public ItemEffect itemEffect07;

    private bool enable09;

    public bool IsItem09Armed
    {
        get => enable09;
        set => enable09 = value;
    }

    private void Awake()
    {
        // 开局禁用武器碰撞体，动画会在攻击时激活它
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    private void OnEnable()
    {
        enable09 = SaveManager.Instance != null && SaveManager.Instance.GetSavedItem09Armed();
        Item09Effect.OnItem09Effect += TriggerItem09Effect;
    }

    private void OnDisable()
    {
        Item09Effect.OnItem09Effect -= TriggerItem09Effect;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 碰撞体由动画的 keyframe 控制开关，能进来说明在攻击判定窗口内
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>()?.OnHurt.Invoke();
            other.gameObject.GetComponent<SecondBossDoor>()?.OnHurt.Invoke();
            channel.Raise3D(SoundEvent.EnemyDie, transform.position);

            if (enable09)
            {
                itemEffect07.ApplyEffect();
                enable09 = false;
            }
        }
    }

    private void TriggerItem09Effect()
    {
        enable09 = true;
    }
}
