using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EliteSpawnItem : MonoBehaviour
{
    private int enable04;
    private string itemsFolderPath = "Prefabs/Items"; // 道具预制体的路径

    private void OnEnable()
    {
        enable04 = SaveManager.Instance != null ? SaveManager.Instance.GetPersistentItemCount(4) : 0;
        Item04Effect.OnItem04Effect += TriggerItem04Effect;
        Inventory.OnInventoryCleared += OnInventoryCleared;
    }

    private void OnDisable()
    {
        Item04Effect.OnItem04Effect -= TriggerItem04Effect;
        Inventory.OnInventoryCleared -= OnInventoryCleared;
    }
    
    public void SpawnRandomItem()
    {
            if (Random.value < 0.04f*enable04)
            {
                // 从指定文件夹加载所有道具预制体
                GameObject[] itemPrefabs = Resources.LoadAll<GameObject>(itemsFolderPath);

                if (itemPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, itemPrefabs.Length);
                    GameObject selectedItemPrefab = itemPrefabs[randomIndex];
                    //GameObject selectedItemPrefab = itemPrefabs[3];
                    // 在宝箱的位置生成道具
                    Instantiate(selectedItemPrefab, transform.position, Quaternion.identity);
                }
            }
    }
    private void TriggerItem04Effect()
    {
        enable04++;
    }

    void OnInventoryCleared()
    {
        enable04 = 0;
    }
}
