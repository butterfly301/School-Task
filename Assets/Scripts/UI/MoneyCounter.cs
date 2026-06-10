using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MoneyCounter : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private PlayerCharacter playerCharacter; 
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        playerCharacter=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacter>();
    }

    private void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            textMesh.text =playerCharacter.money.ToString();
        }
    }
}