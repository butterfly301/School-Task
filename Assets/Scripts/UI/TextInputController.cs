using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using UI;

public class TextInputController : MonoBehaviour
{
    private TMP_InputField inputField; // 在Inspector中分配的InputField组件
    public TextMeshProUGUI hintText;
    public float detectionCooldown = 0.5f; // 检测冷却时间

    private bool isInputActive = false;
    private float lastDetectionTime = 0f;
    private StringBuilder inputBuffer = new StringBuilder();

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        // 初始状态
        inputField.onValueChanged.AddListener(OnInputChanged);
        hintText.text = "  按 T 输入文本";
    }

    void Update()
    {
        // 按下T键切换输入状态
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleInput();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            DealInputMessage();
        }
    }

    void ToggleInput()
    {
        isInputActive = !isInputActive;

        if (isInputActive)
        {
            ActivateInput();
        }
        else
        {
            DeactivateInput();
        }
    }

    void ActivateInput()
    {
        hintText.text = "  请输入文本";
        inputField.text = "";
        inputField.ActivateInputField();
        inputBuffer.Clear();
    }

    void DeactivateInput()
    {
        hintText.text = "  按 T 输入文本";
        inputField.DeactivateInputField();
        isInputActive = false;
    }

    void OnInputChanged(string text)
    {
        // 防止频繁检测
        if (Time.unscaledTime - lastDetectionTime < detectionCooldown)
        {
            return;
        }

        // 将新输入的字符添加到缓冲区
        if (text.Length > inputBuffer.Length)
        {
            inputBuffer.Append(text.Substring(inputBuffer.Length));
        }
        else
        {
            // 如果删除了字符，重置缓冲区
            inputBuffer.Clear();
            inputBuffer.Append(text);
        }
        inputField.text = inputBuffer.ToString();
    }

    void DealInputMessage()
    {
        string fullText = inputBuffer.ToString();
        StartCoroutine(SendSomeMessage(fullText));
        inputField.text = "";
        if (ContainsTurnOnWords(fullText) && ContainsTurnOffWords(fullText))
        {
            inputBuffer.Clear(); // 清空缓冲区
            lastDetectionTime = Time.unscaledTime;
            return;
        }
        if (ContainsTurnOffWords(fullText))
        {
            StartCoroutine(DisableInputTemporarily(2f));
        }

        if (ContainsTurnOnWords(fullText))
        {
            StartCoroutine(EnableInputTemporarily(2f));
        }
        
        inputBuffer.Clear(); // 清空缓冲区
        lastDetectionTime = Time.unscaledTime;
        ToggleInput();
    }

    bool ContainsTurnOffWords(string text)
    {
        // 检测是否包含敏感词
        return text.Contains("闭嘴") || text.Contains("静") || text.Contains("别");
    }

    bool ContainsTurnOnWords(string text)
    {
        return text.Contains("回");
    }

    System.Collections.IEnumerator DisableInputTemporarily(float duration)
    {
        //eactivateInput();
        //hintText.text = "检测到不当用语，输入已禁用 " + duration + " 秒";
        yield return new WaitForSecondsRealtime(duration);
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：好吧，再见");
        FightUIManager.Instance.scrollingDialogueController.ToggleShutUp();
    }
    
    System.Collections.IEnumerator EnableInputTemporarily(float duration)
    {
        //eactivateInput();
        //hintText.text = "检测到不当用语，输入已禁用 " + duration + " 秒";
        yield return new WaitForSecondsRealtime(duration);
        FightUIManager.Instance.scrollingDialogueController.ToggleShutUp();
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("INSPECTOR：哈喽，我回来了");
    }

    System.Collections.IEnumerator SendSomeMessage(string text)
    {
        yield return new WaitForSecondsRealtime(0.5f);
        FightUIManager.Instance.scrollingDialogueController.AddMessageWithScroll("EXECUTOR："+text);
    }
}