using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonClick : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void ButtonClicked()
    {
        button.interactable = false;
        button.interactable = true;
    }
}
