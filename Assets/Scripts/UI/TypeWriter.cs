using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TypeWriter : MonoBehaviour
{
    [Header("打字机设置")]
    [Tooltip("打字速度（字符/秒）")]
    public float charactersPerSecond = 20f;
    [Tooltip("是否在启用时自动开始打字")]
    public bool playOnEnable = true;
    [Tooltip("是否跳过空格")]
    public bool skipSpaces = true;
    
    [Header("高级设置")]
    [Tooltip("标点符号后的额外延迟（秒）")]
    public float punctuationDelay = 0.5f;
    [Tooltip("是否不受TimeScale影响")]
    public bool unscaledTime = true;
    
    private TextMeshProUGUI textComponent;
    private string fullText;
    private Coroutine typingCoroutine;
    
    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        fullText = textComponent.text;
        textComponent.text = "";
    }
    
    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartTyping();
        }
        else
        {
            Invoke("StartTyping",2f);
        }
    }
    
    private void OnDisable()
    {
        StopTyping();
    }
    
    /// <summary>
    /// 开始打字效果
    /// </summary>
    public void StartTyping()
    {
        StopTyping();
        textComponent.text = "";
        typingCoroutine = StartCoroutine(TypeText());
    }
    
    /// <summary>
    /// 停止打字效果并立即显示全部文本
    /// </summary>
    public void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        textComponent.text = fullText;
    }
    
    /// <summary>
    /// 打字效果协程
    /// </summary>
    private IEnumerator TypeText()
    {
        textComponent.text = "";
        float delay = 1f / charactersPerSecond;
        
        for (int i = 0; i < fullText.Length; i++)
        {
            char currentChar = fullText[i];
            textComponent.text += currentChar;
            
            // 检查是否需要额外延迟（标点符号后）
            if (i < fullText.Length - 1 && IsPunctuation(currentChar))
            {
                yield return GetWaitObject(punctuationDelay);
            }
            else if (!(skipSpaces && char.IsWhiteSpace(currentChar)))
            {
                yield return GetWaitObject(delay);
            }
        }
        
        typingCoroutine = null;
    }
    
    /// <summary>
    /// 根据设置返回适当的等待对象
    /// </summary>
    private object GetWaitObject(float seconds)
    {
        return unscaledTime ? 
            new WaitForSecondsRealtime(seconds) : 
            (object)new WaitForSeconds(seconds);
    }
    
    /// <summary>
    /// 检查字符是否是标点符号
    /// </summary>
    private bool IsPunctuation(char character)
    {
        return character == '.' || character == '!' || character == '?' || 
               character == ',' || character == ';' || character == ':';
    }
    
    /// <summary>
    /// 立即完成打字效果
    /// </summary>
    public void FinishTyping()
    {
        StopTyping();
        textComponent.text = fullText;
    }
    
    /// <summary>
    /// 更新显示文本（会重置打字效果）
    /// </summary>
    public void SetText(string newText)
    {
        fullText = newText;
        if (gameObject.activeInHierarchy && playOnEnable)
        {
            StartTyping();
        }
        else
        {
            textComponent.text = fullText;
        }
    }
}