using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScrollingDialogueController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private Transform messagesContainer;
    [SerializeField] private ScrollRect scrollRect;
    
    [Header("Settings")]
    [SerializeField] private int maxVisibleMessages = 4;
    [SerializeField] private float scrollDuration = 0.3f;
    private bool shutUp;
    
    [System.Serializable]
    public class TimedMessage
    {
        public GameObject messageObj;
        public float destroyTime;
        public bool isBeingDestroyed;
    }
    
    private Queue<GameObject> activeMessages = new Queue<GameObject>();
    private float containerSpacing;
    
    private VerticalLayoutGroup layoutGroup;
    private void Start()
    {
        // 获取容器间距（Vertical Layout Group的spacing）
        if (messagesContainer.TryGetComponent<VerticalLayoutGroup>(out var verticalLayoutGroup))
        {
            containerSpacing = verticalLayoutGroup.spacing;
        }
        
        layoutGroup = messagesContainer.GetComponent<VerticalLayoutGroup>();
        
        shutUp = false;
        // 测试代码
        //StartCoroutine(TestMessages());
    }
    
    private List<TimedMessage> timedMessages = new List<TimedMessage>();
    private float messageLifetime = 10f; // 10秒后销毁
    
    private void Update()
    {
        CheckExpiredMessages();
    }

    IEnumerator TestMessages()
    {
        AddMessageWithScroll("INSPECTOR:欢迎来到游戏！");
        yield return new WaitForSeconds(1f);
        AddMessageWithScroll("INSPECTOR:这是一个滚动对话框示例");
        yield return new WaitForSeconds(1f);
        AddMessageWithScroll("INSPECTOR:最多同时显示四条消息");
        yield return new WaitForSeconds(1f);
        AddMessageWithScroll("INSPECTOR:新消息会顶掉旧消息");
        yield return new WaitForSeconds(1f);
        AddMessageWithScroll("INSPECTOR:这是第五条消息，会滚动显示");
    }
    
    public void AddMessageWithScroll(string text)
    {
        if(shutUp) return;
        // 如果达到最大消息数，移除最早的一条
        /*if (activeMessages.Count >= maxVisibleMessages)
        {
            GameObject oldMessage = activeMessages.Dequeue();
            Destroy(oldMessage);
        }*/
        
        if (activeMessages.Count >= maxVisibleMessages)
        {
            StartCoroutine(RemoveTopMessageAndScroll());
        }
        
        // 创建新消息
        GameObject newMessage = Instantiate(messagePrefab, messagesContainer);
        newMessage.GetComponent<TextMeshProUGUI>().text = text;
        activeMessages.Enqueue(newMessage);
        
        // 启动滚动动画
        StartCoroutine(SmoothScrollToBottom());
        
        // 添加计时
        timedMessages.Add(new TimedMessage(){
            messageObj = newMessage,
            destroyTime = Time.time + messageLifetime,
            isBeingDestroyed = false
        });
    }
    
    private IEnumerator SmoothScrollToBottom()
    {
        yield return new WaitForEndOfFrame(); // 等待UI布局更新
        
        // 计算内容总高度（包括间距）
        float contentHeight = 0f;
        foreach (Transform child in messagesContainer)
        {
            if (child.TryGetComponent<RectTransform>(out var rect))
            {
                contentHeight += rect.rect.height;
                contentHeight += containerSpacing;
            }
        }
        
        // 计算视口和内容的相对高度
        float viewportHeight = scrollRect.viewport.rect.height;
        float scrollDistance = Mathf.Max(0, contentHeight - viewportHeight);
        
        // 如果不需要滚动，直接返回
        if (scrollDistance <= 0)
        {
            scrollRect.verticalNormalizedPosition = 0;
            yield break;
        }
        
        // 动画滚动
        float startPos = scrollRect.verticalNormalizedPosition;
        float endPos = 0f;
        float elapsed = 0f;
        
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scrollDuration);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPos, endPos, t);
            yield return null;
        }
        
        scrollRect.verticalNormalizedPosition = endPos;
    }
    
    private void OnDestroy()
    {
        // 场景销毁时清理所有消息
        ClearAllMessages();
    }

    public void ClearAllMessages()
    {
        StopAllCoroutines(); // 停止所有滚动动画

        // 销毁所有活跃消息
        foreach (var message in activeMessages)
        {
            if (message != null)
            {
                Destroy(message);
            }
        }
        activeMessages.Clear();

        // 重置滚动位置
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1;
        }
    }
    
    private IEnumerator RemoveTopMessageAndScroll()
    {
        // 1. 移除顶部消息
        GameObject oldMessage = activeMessages.Dequeue();
    
        // 2. 计算下一条消息的目标位置（顶部）
        if (activeMessages.Count > 0 && oldMessage.TryGetComponent<RectTransform>(out var _))
        {
            float messageHeight = ((RectTransform)oldMessage.transform).rect.height + layoutGroup.spacing;
        
            // 3. 开始移除和滚动动画
            Destroy(oldMessage);
            yield return StartCoroutine(SmoothScrollToPosition(messageHeight));
        }
        else
        {
            Destroy(oldMessage);
        }
    }

    private IEnumerator SmoothScrollToPosition(float targetYOffset)
    {
        float startPos = scrollRect.content.anchoredPosition.y;
        float endPos = startPos - targetYOffset; // 向上滚动
    
        float elapsed = 0f;
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scrollDuration);
            float newY = Mathf.Lerp(startPos, endPos, t);
            scrollRect.content.anchoredPosition = new Vector2(
                scrollRect.content.anchoredPosition.x, 
                newY
            );
            yield return null;
        }
    }
    
    private void CheckExpiredMessages()
    {
        for (int i = timedMessages.Count - 1; i >= 0; i--)
        {
            var msg = timedMessages[i];
            if (msg == null || msg.messageObj == null)  // 检查 msg 和 msg.messageObj 是否有效
            {
                timedMessages.RemoveAt(i);  // 清理无效条目
                continue;
            }

            if (Time.time >= msg.destroyTime && !msg.isBeingDestroyed)
            {
                msg.isBeingDestroyed = true;
                StartCoroutine(DestroyMessageWithScroll(msg));
            }
        }
    }

    private IEnumerator DestroyMessageWithScroll(TimedMessage msg)
    {
        if (msg == null || msg.messageObj == null)  // 再次检查
        {
            timedMessages.Remove(msg);
            yield break;
        }

        // 1. 从队列中移除
        var newQueue = new Queue<GameObject>();
        foreach (var m in activeMessages)
        {
            if (m != msg.messageObj) 
                newQueue.Enqueue(m);
        }
        activeMessages = newQueue;

        // 2. 从列表中移除
        timedMessages.Remove(msg);

        // 3. 如果是顶部消息，触发滚动
        if (msg.messageObj.transform.GetSiblingIndex() == 0)
        {
            yield return StartCoroutine(ScrollAfterTopRemoval());
        }

        // 4. 销毁对象
        Destroy(msg.messageObj);
    }

    private IEnumerator ScrollAfterTopRemoval()
    {
        if(activeMessages.Count == 0) yield break;
        
        // 计算滚动距离（消息高度+间距）
        float scrollDistance = 0f;
        if(activeMessages.Peek().TryGetComponent<RectTransform>(out var rect))
        {
            scrollDistance = rect.rect.height + layoutGroup.spacing;
        }
        
        // 执行滚动动画
        Vector2 startPos = scrollRect.content.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, scrollDistance);
        
        float elapsed = 0f;
        while(elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            scrollRect.content.anchoredPosition = Vector2.Lerp(
                startPos, 
                endPos, 
                elapsed / scrollDuration
            );
            yield return null;
        }
    }

    public void ToggleShutUp()
    {
        shutUp = !shutUp;
    }
}