using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UICinematicController : MonoBehaviour
{
    public Teleporter teleporter;
    [Header("时间控制")]
    [SerializeField] private float playDuration = 2f;  // 淡出持续时间

    void Start()
    {
        StartCoroutine(PlayCinematicSequence());
    }

    IEnumerator PlayCinematicSequence()
    {
        yield return new WaitForSeconds(playDuration);
        // 加载下一场景
        teleporter.TriggerAction();
    }
}