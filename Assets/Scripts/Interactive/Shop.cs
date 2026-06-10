using System;
using UI;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

public class Shop : Interactive
{
    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public override void MakeSomeReaction()
    {
        base.MakeSomeReaction();
        if (!FixedUIManager.Instance.shopPanel.activeSelf)
        {
            FixedUIManager.Instance.ToggleShopPanel();
        }
    }
    
}
