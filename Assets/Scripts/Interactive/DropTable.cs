using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "DropSystem/DropTable")]
public class DropTable : ScriptableObject
{
    public List<DropItem> items = new();

    public float GetTotalWeight(float timeElapsed)
    {
        return items.Sum(i => i.GetCurrentWeight(timeElapsed));
    }

    public DropItem GetDropItem(float timeElapsed)
    {
        float total = GetTotalWeight(timeElapsed);
        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var item in items)
        {
            cumulative += item.GetCurrentWeight(timeElapsed);
            if (rand <= cumulative)
                return item;
        }

        return null;
    }
}
