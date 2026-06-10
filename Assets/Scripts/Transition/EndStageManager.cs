using UI;
using UnityEngine;

public class EndStageManager : MonoBehaviour
{
    private void Start()
    {
        FixedUIManager.Instance.ShowSummaryPanel("Success");
        
        // 获取所有 ItemHolder 并重置
        ItemHolder[] itemHolders = FindObjectsOfType<ItemHolder>();
        
        // 检查是否有 ItemHolder
        if (itemHolders.Length > 0)
        {
            foreach (ItemHolder holder in itemHolders)
            {
                if (holder != null)
                {
                    holder.ResetItemInstance();  // 重置实例
                }
            }
            
        }
    }
    
    
}