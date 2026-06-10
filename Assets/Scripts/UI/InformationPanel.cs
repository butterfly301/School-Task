using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemInformation;
    public Image itemSprite;
    public TextMeshProUGUI itemPrice;

    public  void SetItemDiscription (string name, string information, Sprite sprite, int price)
    {
        this.itemName.text = name;
        this.itemInformation.text = information;
        this.itemSprite.sprite = sprite;
        if(itemPrice != null)
            this.itemPrice.text = "$" + price;
    }
}
