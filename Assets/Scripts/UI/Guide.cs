using System;
using System.Collections;
using UI;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public string[] guideTexts;
    public float[] interval;

    public void StartShowGuide()
    {
        StartCoroutine(ShowGuide());
    }

    IEnumerator ShowGuide()
    {
        for (int i = 0; i < guideTexts.Length; i++)
        {
            FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll(guideTexts[i]);
            yield return new WaitForSeconds(interval[i]);
        }
    }
}
