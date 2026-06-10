using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenModeSwitcher : MonoBehaviour
{
    public Image[] buttons;
    public Sprite normalButton;
    public Sprite selectedButton;

    private void Start()
    {
        // 获取当前分辨率
        int currentWidth = Screen.width;
        bool isFullscreen = Screen.fullScreen;
        if (isFullscreen)
        {
            SwitchButtons(0);
        }
        else
        {
            switch (currentWidth)
            {
                case 1920:
                    SwitchButtons(1);
                    break;
                case 1600:
                    SwitchButtons(2);
                    break;
                case 1280:
                    SwitchButtons(3);
                    break;
            }
        }
        
    }

    public void SwitchButtons(int index)
    {
        foreach (var button in buttons)
        {
            button.sprite = normalButton;
        }
        buttons[index].sprite = selectedButton;
    }
    
    // 切换到全屏
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        // 或者更精细的控制：
        // Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
    }
    

    // 切换到窗口模式（指定分辨率）
    public void SetWindowed(string widthAndHeight)
    {
        string[] numbers = widthAndHeight.Split('*');
        int width=Convert.ToInt32(numbers[0]);
        int height=Convert.ToInt32(numbers[1]);
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
    }
}