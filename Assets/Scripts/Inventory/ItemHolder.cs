using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public ItemData itemData; // �?Inspector 中拖入对应的 SO

    [HideInInspector] public ItemInstance itemInstance; // 运行时实�?

    private void Start()
    {
        InitializeItemInstance();
    }

    // 初始化或重置 itemInstance
    public void ResetItemInstance()
    {
        InitializeItemInstance();
    }

    private void InitializeItemInstance()
    {
        if (itemData == null)
        {
            return;
        }
        itemInstance = new ItemInstance(itemData);
    }
}
