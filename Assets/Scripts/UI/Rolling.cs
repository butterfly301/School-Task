using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollingUI : MonoBehaviour
{
    [Header("UI")]
    public Button RB;                            // 右按钮
    public Button LB;                            // 左按钮
    public GameObject optionPrefab;              // 商品预制体
    public Transform OptionGroup;                // 商品父节点
    public InformationPanel infoPanel;           // 展示面板

    [Header("商品数据 (Scriptable Objects)")]
    public ItemData[] itemList;                  // 存储于 ScriptableObject 的商品数据
    [Range(1, 10)] public int displayCount = 5;  // 显示的商品数量（最大10）

    private List<ItemData> selectedItems = new List<ItemData>();

    [Header("展示参数")]
    public float R = 500f;                       // 旋转半径
    public float yOffset = 20f;                  // y 轴偏移
    [Range(0, 1)] public float minAlpha = 0.3f;  // 最小透明度
    [Range(1, 5)] public float firstS = 1.5f;    // 选中缩放倍数
    [Range(0, 1)] public float minS = 0.7f;      // 最小缩放倍数
    public float smoothSTime = 0.1f;             // 缩放平滑时间

    private List<Transform> options = new List<Transform>();
    private List<CanvasGroup> cgs = new List<CanvasGroup>();
    private Dictionary<Transform, Vector3> posDic = new Dictionary<Transform, Vector3>();

    private Coroutine[] scaleCoroutines;
    private Coroutine currentMove;
    private int first = 0;
    

    private void OnEnable()
    {
        SelectRandomItems();   // 随机选取 displayCount 个商品
        InitItems();
        // InitPositions();
        UpdateAll();

        LB.onClick.AddListener(ClickLeft);
        RB.onClick.AddListener(ClickRight);
    }

    // 从 itemList 中随机抽取 displayCount 个
    void SelectRandomItems()
    {
        selectedItems.Clear();
        if (itemList == null || itemList.Length == 0)
            return;

        var pool = itemList.Where(i => i != null).ToList();
        int count = Mathf.Clamp(displayCount, 1, pool.Count);
        selectedItems = pool.OrderBy(x => Random.value).Take(count).ToList();
    }

    void InitItems()
    {
        // 1. 清空旧实例
        foreach (Transform child in OptionGroup)
            Destroy(child.gameObject);
        options.Clear();
        cgs.Clear();

        // 2. 实例化并赋值
        foreach (var item in selectedItems)
        {
            // a) 实例化
            GameObject goodToBuy = Instantiate(optionPrefab, OptionGroup);
            goodToBuy.name = item.itemName;

            // b) 拿到 Image 组件，赋 sprite
            Image img = goodToBuy.transform
                .Find("ItemImage")
                .GetComponent<Image>();
            img.sprite = item.itemSprite;

            // c) 拿到名称 TextMeshPro，赋 name
            TextMeshProUGUI nameText = goodToBuy.transform
                .Find("ItemName")
                .GetComponent<TextMeshProUGUI>();
            nameText.text = item.itemName;

            // d) 拿到信息 TextMeshPro，赋 description
            TextMeshProUGUI infoText = goodToBuy.transform
                .Find("InformationText")
                .GetComponent<TextMeshProUGUI>();
            infoText.text = item.itemInformation;

            // e) 缓存 transform 和 CanvasGroup
            options.Add(goodToBuy.transform);
            cgs.Add(goodToBuy.GetComponent<CanvasGroup>());
            
            Purchase purchase = goodToBuy.GetComponent<Purchase>();
            purchase.itemData = item;
            
        }

        // 3. 初始化缩放协程数组 & 重置选中索引
        scaleCoroutines = new Coroutine[options.Count];
        first = 0;
        
        for (int i = 0; i < options.Count; i++)
            options[i].gameObject.SetActive(i == first);  // 只显示第一个
    }

    // void InitPositions()
    // {
    //     int count = options.Count;
    //     for (int i = 0; i < count; i++)
    //     {
    //         float angle = (360f / count) * i * Mathf.Deg2Rad;
    //         float x = Mathf.Sin(angle) * R;
    //         float z = -Mathf.Cos(angle) * R;
    //         float y = (i > count / 2f) ? (count - i) * yOffset : i * yOffset;
    //
    //         Vector3 pos = new Vector3(x, y, z);
    //         options[i].localPosition = pos;
    //         posDic[options[i]] = pos;
    //     }
    // }

    void UpdateAll()
    {
        UpdateAlpha();
        // UpdateScale();
        UpdateInfoPanel();
    }

    void UpdateAlpha()
    {
        for (int i = 0; i < cgs.Count; i++)
        {
            cgs[i].alpha = (i == first) ? 1f : 0f;
        }
    }

    // void UpdateScale()
    // {
    //     float startZ = -R;
    //     for (int i = 0; i < options.Count; i++)
    //     {
    //         float distanceZ = Mathf.Abs(options[i].localPosition.z - startZ);
    //         float targetScale = 1 - (distanceZ / (2 * R)) * (1 - minS);
    //         if (i == first)
    //             targetScale = firstS;
    //
    //         if (scaleCoroutines[i] != null)
    //             StopCoroutine(scaleCoroutines[i]);
    //
    //         scaleCoroutines[i] = StartCoroutine(SmoothScale(options[i], targetScale));
    //     }
    // }

    // IEnumerator SmoothScale(Transform tf, float target)
    // {
    //     float velocity = 0;
    //     while (Mathf.Abs(tf.localScale.x - target) > 0.01f)
    //     {
    //         float scale = Mathf.SmoothDamp(tf.localScale.x, target, ref velocity, smoothSTime);
    //         tf.localScale = Vector3.one * scale;
    //         yield return null;
    //     }
    //     tf.localScale = Vector3.one * target;
    // }

    void UpdateInfoPanel()
    {
        if (selectedItems == null || selectedItems.Count == 0) return;
        first = Mathf.Clamp(first, 0, selectedItems.Count - 1);
        var item = selectedItems[first];
        /*infoPanel=optionPrefab.GetComponent<InformationPanel>();
        infoPanel.SetItemDiscription(item.itemName, item.itemInformation, item.itemSprite, item.price);*/
    }

    void ClickLeft() => StartCoroutine(MoveLeft());
    void ClickRight() => StartCoroutine(MoveRight());

    IEnumerator MoveLeft()
    {
        if (currentMove != null) yield return currentMove;

        // 切换显示状态
        options[first].gameObject.SetActive(false);
        first = (first == 0) ? options.Count - 1 : first - 1;
        options[first].gameObject.SetActive(true);

        yield return null;
        UpdateAll();
    }

    IEnumerator MoveRight()
    {
        if (currentMove != null) yield return currentMove;

        options[first].gameObject.SetActive(false);
        first = (first + 1) % options.Count;
        options[first].gameObject.SetActive(true);

        yield return null;
        UpdateAll();
    }


}
