using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterForLoadStage : MonoBehaviour
{
    public Teleporter teleporter;
    private void Start()
    {
        StartCoroutine(ExecuteAfterDelay(6));
    }

    IEnumerator ExecuteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        teleporter.TriggerAction();
    }
}
