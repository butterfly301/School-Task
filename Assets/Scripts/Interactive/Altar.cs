using UI;
using UnityEngine;
using UnityEngine.Events;

public class Altar : Interactive
{
    private bool isTiped;
    public DropSystem dropSystem;
    private string itemsFolderPath = "Prefabs/Items"; // 道具预制体的路径
    private Vector3 offset = new Vector3(0, 0.5f, 0);
    
    
    public void SpawnRandomItem()
    {
        // 从指定文件夹加载所有道具预制体
        /* GameObject[] itemPrefabs = Resources.LoadAll<GameObject>(itemsFolderPath);

         if (itemPrefabs.Length > 0)
         {
             int randomIndex = Random.Range(0, itemPrefabs.Length);
             GameObject selectedItemPrefab = itemPrefabs[randomIndex];
             //GameObject selectedItemPrefab = itemPrefabs[3];
             // 在宝箱的位置生成道具
             Instantiate(selectedItemPrefab, transform.position+offset, Quaternion.identity);
         }*/
        dropSystem.Drop(transform.position+offset);
    }
    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            if (!isTiped)
            {
                FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：使用自动贩卖机需要献出秒剩余时间");
                isTiped = true;
            }
            
            FightUIManager.Instance.visionPanel.onTimePriceTipEnable(transform);
        }
    }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            FightUIManager.Instance.visionPanel.onTimePriceTipKeep(transform); 
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.CompareTag("Player") && playerState != null && playerState.isAlive)
        {
            FightUIManager.Instance.visionPanel.onTimePriceTipDisable();
        }
    }
    public void Awake()
    {
        dropSystem=FindObjectOfType<DropSystem>();
    }
}
