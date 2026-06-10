using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;
    
    public GameObject informationPanelForBagPanel;

    public int countPerRow;

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
        inventory.OnItemListChanged += Inventory_OnItemListChanged; // 订阅物品列表变更事件
        RefreshInventoryItems(); // 初始化时刷新物品
    }

    // 刷新显示背包的所有物品
    private void RefreshInventoryItems()
    {
        itemSlotContainer=transform.Find("ItemSlotContainer").GetComponent<RectTransform>();
        itemSlotTemplate = itemSlotContainer.Find("ItemSlotTemplate").GetComponent<RectTransform>();
        // 先收集所有需要销毁的子物体（排除模板）
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in itemSlotContainer)
        {
            if (child != itemSlotTemplate && child.gameObject != itemSlotTemplate.gameObject)
            {
                childrenToDestroy.Add(child.gameObject);
            }
        }

        // 统一销毁
        foreach (GameObject child in childrenToDestroy)
        {
            if (child != null)
            {
                Destroy(child);
            }
        }
        
        // 重新生成物品槽
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 130f;

        // 遍历背包中的所有物品
        foreach (var itemInstance in inventory.GetItemList())
        {
            var itemData = itemInstance.baseData; // 获取物品的原始数据

            RectTransform itemSlotRectTransform =
                Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);

            Image itemImage = itemSlotRectTransform.GetComponentInChildren<Image>();
            itemImage.sprite = itemData.itemSprite;
            
            ItemHolder itemHolder = itemSlotRectTransform.GetComponentInChildren<ItemHolder>();
            itemHolder.itemData = itemData;

            TextMeshProUGUI text = itemSlotRectTransform.GetComponentInChildren<TextMeshProUGUI>();
            text.text = itemInstance.currentAmount > 1 ? "×" + itemInstance.currentAmount : ""; // 显示数量

            x++;
            if (x > countPerRow)
            {
                x = 0;
                y--;
            }
        }
    }

    // 物品列表发生变化时更新 UI
    private void Inventory_OnItemListChanged(object sender, EventArgs e)
    {
        RefreshInventoryItems();
    }
    
    public void ShowInformationPanel(ItemData itemData)
    {
        if (informationPanelForBagPanel != null)
        {
            informationPanelForBagPanel.GetComponent<InformationPanel>()
                ?.SetItemDiscription(itemData.itemName, itemData.itemInformation, itemData.itemSprite, itemData.price);
            informationPanelForBagPanel.SetActive(true);
        }
    }
    
    public void HideInformationPanel()
    {
        if(informationPanelForBagPanel != null)
            informationPanelForBagPanel.SetActive(false);
    }

    
    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnItemListChanged -= Inventory_OnItemListChanged;
    }
}