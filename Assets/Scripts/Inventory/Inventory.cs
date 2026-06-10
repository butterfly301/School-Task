using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public event EventHandler OnItemListChanged;
    public static event Action OnInventoryCleared;

    private List<ItemInstance> itemList;

    public Inventory()
    {
        itemList = new List<ItemInstance>();
    }

    // 添加物品到背包
    public void AddItem(ItemData itemData)
    {
        if (itemData.isStackable()) // 如果是可堆叠的物品
        {
            bool found = false;
            foreach (ItemInstance item in itemList)
            {
                if (item.GetItemID() == itemData.itemID)
                {
                    item.currentAmount += 1; // 增加数量
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                itemList.Add(new ItemInstance(itemData)); // 添加新的物品实例
            }
        }
        else
        {
            itemList.Add(new ItemInstance(itemData)); // 不可堆叠的物品，直接添加
        }

        OnItemListChanged?.Invoke(this, EventArgs.Empty); // 通知 UI 更新
    }

    // 获取所有物品
    public List<ItemInstance> GetItemList()
    {
        return itemList;
    }

    // 重置背包内容
    public void ResetInventory()
    {
        itemList.Clear();
        OnItemListChanged?.Invoke(this, EventArgs.Empty);
        OnInventoryCleared?.Invoke(); // 触发静态事件
    }
}