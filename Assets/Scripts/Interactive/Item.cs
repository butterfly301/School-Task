using System;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Item : Interactive
{
    public ItemData itemData;
    private Inventory inventory;
    
    private float radius = 4f; // 圆的半径
    private float moveSpeed = 20f; // 移动速度

    private Vector3 targetPosition; // 目标位置
    private bool isMoving; // 是否正在移动
    private bool isChasingPlayer;

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }

    protected override void Start()
    {
        base.Start();
        // 初始化目标位置
        SetRandomTargetPosition();
    }

    void Update()
    {
        if (isMoving)
        {
            // 使用 Lerp 平滑移动
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 检查是否接近目标位置
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // 停止移动
            }
        }
        if (isChasingPlayer)
        {
            targetPosition = player.transform.position;
        }
        if (canInteract)
        {
            Interact();
        }
        //HideInfo();
        
    }

    void SetRandomTargetPosition()
    {
        // 随机生成一个角度（0到360度）
        float randomAngle = Random.Range(180f, 270f);
        
        // 将角度转换为弧度
        float angleInRadians = randomAngle * Mathf.Deg2Rad;
        
        // 计算圆上的位置（半径为2）
        float x = radius * Mathf.Cos(angleInRadians);
        float z = radius * Mathf.Sin(angleInRadians);

        // 设置目标位置（以自身为圆心）
        targetPosition = transform.position + new Vector3(x, 0, z);

        // 开始移动
        isMoving = true;
    }
    
    public override void MakeSomeReaction()
    {
        base.MakeSomeReaction();
        // 停止当前的随机移动
        isMoving = false;
        isChasingPlayer = true;
        
        if (player != null)
        {
            // 设置目标位置为玩家位置
            targetPosition = player.transform.position;

            // 开始移动
            isMoving = true;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FightUIManager.Instance.visionPanel.onInteractTipDisable();
            FightUIManager.Instance.HideInformationPanel();
            FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：拾取道具:"+itemData.itemName);
            FixedUIManager.Instance.SetItemInventory(this);
            inventory.AddItem(itemData);
            itemData.itemEffect.ApplyEffect();
            Destroy(gameObject);
        }
    }
    /*protected void HideInfo()
    {
        if(isGone)
        {
            UIManager.Instance.HideInformationPanel();
        }
    }*/
}
