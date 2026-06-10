using UnityEngine;

public class ResetPlayerState : MonoBehaviour
{
    public void OnEnable()
    {
        var playerStateController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateController>();
        playerStateController.ResetPlayer();
        GameStatsManager.Instance.ResetStats();
    }
}
