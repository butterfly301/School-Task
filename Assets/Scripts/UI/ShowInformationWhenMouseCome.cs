using System;
using UnityEngine;

public class ShowInformationWhenMouseCome : MonoBehaviour
{
    private UI_Inventory bagPanel;
    private ItemHolder itemHolder;

    private void Start()
    {
        itemHolder = GetComponent<ItemHolder>();
        bagPanel = GetComponentInParent<UI_Inventory>();
    }

    public void OnPointEnter()
    {
        bagPanel.ShowInformationPanel(itemHolder.itemData);
    }

    public void OnPointExit()
    {
        bagPanel.HideInformationPanel();
    }
}
