using UnityEngine;
using TMPro;

public class ShieldCounter : MonoBehaviour
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
        textMesh.text =playerCharacter.shield.ToString();
    }
}
