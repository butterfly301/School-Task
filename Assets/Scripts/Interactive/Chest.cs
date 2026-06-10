using System;
using UI;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

public class Chest : Interactive
{
    public int requireMoney;
    private string itemsFolderPath = "Prefabs/Items"; // 道具预制体的路径
    private bool isTiped;
    private bool isOpened; // 宝箱是否已开启
    private int canDouble=0;
    private Vector3 offset = new Vector3(0, 0.5f, 0);
    public Transform lockTransform;
    public DropSystem dropSystem;
    private PlayerCharacter playerCharacter;
    [SerializeField] private AudioEventChannel channel;
    
    private void OnEnable()
    {
        Item10Effect.OnItem10Effect += AddSpawnItemCount;
        Inventory.OnInventoryCleared += OnInventoryCleared;
    }

    private void OnDisable()
    {
        Item10Effect.OnItem10Effect -= AddSpawnItemCount;
        Inventory.OnInventoryCleared -= OnInventoryCleared;
    }

    /*public override void Interact()
    {
        // 检查是否按下右键
        if (Input.GetKeyDown(KeyCode.E) &&(!isOpened)&&playerCharacter.money >= requireMoney) 
        {
            //不同交互物要做不同的事情
            MakeSomeReaction();
        }
    }*/
  

    public override void MakeSomeReaction()
    {
        if (isOpened || playerCharacter.money < requireMoney) return;
        base.MakeSomeReaction();
        playerCharacter = player.GetComponent<PlayerCharacter>();
        playerCharacter.money -= requireMoney;
        isOpened = true;
        channel.Raise3D(SoundEvent.BoxOpen, transform.position);
       
        SpawnRandomItem();
        
        FightUIManager.Instance.visionPanel.onInteractTipDisable();
        FightUIManager.Instance.visionPanel.onPriceTipDisable();
        
        
    }

     private void SpawnRandomItem()
     {
        // 从指定文件夹加载所有道具预制体
        /* GameObject[] itemPrefabs = Resources.LoadAll<GameObject>(itemsFolderPath);

         if (itemPrefabs.Length > 0)
         {
             if (canDouble==0)// 随机选择一个道具，如果有道具10则生成两个
             {
                 int randomIndex = Random.Range(0, itemPrefabs.Length);
                 GameObject selectedItemPrefab = itemPrefabs[randomIndex];
                 //GameObject selectedItemPrefab = itemPrefabs[3];
                 // 在宝箱的位置生成道具
                 Instantiate(selectedItemPrefab, transform.position+offset, Quaternion.identity);
             }
             else if (canDouble!=0)
             {
                 for (int i = 0; i <= canDouble+1; i++)
                 {
                     int randomIndex = Random.Range(0, itemPrefabs.Length);
                     GameObject selectedItemPrefab = itemPrefabs[randomIndex];
                     //GameObject selectedItemPrefab = itemPrefabs[3];
                     // 在宝箱的位置生成道具
                     Instantiate(selectedItemPrefab, transform.position, Quaternion.identity);
                 }
                 canDouble=0;
             }
         }*/
        if (canDouble == 0)
        {
            dropSystem.Drop(transform.position + offset);
        } else if (canDouble != 0)
        {
            for (int i = 0; i < canDouble + 1; i++)
            {
                dropSystem.Drop(transform.position + offset);
            }
        }
    
     }

    private void AddSpawnItemCount()
    {
        canDouble++;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpened && playerState != null && playerState.isAlive)
        {
            FightUIManager.Instance.visionPanel.onPriceTipEnable(lockTransform);
            if (!isTiped)
            {
                FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：打开箱子需要献出20金币");
                isTiped = true;
            }
            playerCharacter = player.GetComponent<PlayerCharacter>();
            if (playerCharacter.money >= requireMoney)
            {
                canInteract = true;
                FightUIManager.Instance.visionPanel.onInteractTipEnable(lockTransform);
            }
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            FightUIManager.Instance.visionPanel.onPriceTipKeep(lockTransform);
            playerCharacter = player.GetComponent<PlayerCharacter>();
            if (playerCharacter.money >= requireMoney)
                FightUIManager.Instance.visionPanel.onInteractTipKeep(lockTransform);
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {   
            canInteract = false;
            FightUIManager.Instance.visionPanel.onInteractTipDisable();
            FightUIManager.Instance.visionPanel.onPriceTipDisable();
        }
    }
    
    /*private bool IsMouseOverThisChest()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider == GetComponent<Collider>();
        }
        return false;
    }*/
    
    private void OnInventoryCleared()
    {
        canDouble = 0;
    }
    private void Awake()
    {
        dropSystem=FindObjectOfType<DropSystem>();
    }
}
