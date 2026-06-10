using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public string itemInformation;
    public Sprite itemSprite;
    public int amount;
    public int price;
    public ItemEffect itemEffect;

    public bool isStackable()
    {
        switch (itemID)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 6:
            case 10:
                //return true;
            case 5:
            case 7:
            case 8:
            case 9:
                return true;
            default:
                return false;
            
        }
    }

    public void ResetAmount()
    {
        amount = 1;
    }
}