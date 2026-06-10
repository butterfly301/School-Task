using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class Purchase : MonoBehaviour
{
    public Button buyButton;
    public ItemData itemData;            // 要购买的物品
    private int price;              // 物品价格
    public Inventory inventory;
    private bool hasBought = false;
    private InformationPanel infoPanel;
    private PlayerCharacter playerCharacter;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
        UpdateButtonState();
        price = itemData.price;
        playerCharacter = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
        infoPanel = GetComponent<InformationPanel>();
        infoPanel.SetItemDiscription(itemData.itemName,itemData.itemInformation,itemData.itemSprite,itemData.price);
        
    }

    public void OnBuyClicked()
    {
        if (hasBought) return;
        if (playerCharacter.money >= price)
        {
            playerCharacter.money -= price;                // 扣除金币
            FixedUIManager.Instance.SetPurchaseInventory(this);
            itemData.itemEffect.ApplyEffect();
            inventory.AddItem(itemData);
            hasBought = true;
            UpdateButtonState();
        }
    }
    
    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;
    }
    
    void UpdateButtonState()
    {
        buyButton.interactable = !hasBought;
        if(buyButton.interactable)
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "购买";
        else
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "已售完";
    }

}