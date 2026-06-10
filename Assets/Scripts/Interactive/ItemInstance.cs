using UnityEngine;

[System.Serializable]
public class ItemInstance
{
    public ItemData baseData;       // 引用原始 ScriptableObject
    public int currentAmount;       // 当前数量（运行时独立）
    public int currentPrice;        // 当前价格（可变）

    public ItemInstance(ItemData data)
    {
        baseData = data;
        ResetState();
    }

    public void ResetState()
    {
        currentAmount = baseData.amount;
        currentPrice = baseData.price;
    }

    // 方便访问信息
    public string GetItemName() => baseData.itemName;
    public string GetItemInfo() => baseData.itemInformation;
    public Sprite GetItemSprite() => baseData.itemSprite;
    public bool IsStackable() => baseData.isStackable();
    public int GetItemID() => baseData.itemID;
}