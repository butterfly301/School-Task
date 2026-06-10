using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;  // 单例模式
    public GameObject tooltipPanel;  // 整个提示框的 GameObject
    public Image itemIcon;           // 物品图标
    public Text itemName;            // 物品名称
    public Text itemInformation;     // 物品描述

    void Awake()
    {
        instance = this;
        
        tooltipPanel.SetActive(false); // 初始隐藏
    }

    // 显示提示框
    public void ShowTooltip(Item item)
    {
        if (item == null) return;

        // 更新内容
        itemIcon.sprite = item.itemData.itemSprite;
        itemName.text = item.itemData.itemName;
        itemInformation.text = item.itemData.itemInformation;

        tooltipPanel.SetActive(true);
    }

    // 隐藏提示框
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}