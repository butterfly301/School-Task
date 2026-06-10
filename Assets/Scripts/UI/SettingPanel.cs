using System;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    public GameObject[] images;
    public GameObject[] panels;

    private void Start()
    {
        SwitchSetting(0);
    }

    public void SwitchSetting(int index)
    {
        foreach (var image in images)
        {
            image.SetActive(false);
        }

        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        images[index].SetActive(true);
        panels[index].SetActive(true);
    }
}
