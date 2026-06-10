using UnityEngine;

[System.Serializable]
public class DropItem
{
    public string itemName;
    public GameObject prefab;

    [Min(0)] public float baseWeight = 1f; // 每个道具初始化就有不同权重

    public bool dynamicWeightEnabled = false;
    public AnimationCurve weightOverTime = AnimationCurve.Linear(0, 1, 60, 1);

    public float GetCurrentWeight(float timeElapsed)
    {
        if (!dynamicWeightEnabled)
            return baseWeight;

        float factor = weightOverTime.Evaluate(timeElapsed);
        return baseWeight * Mathf.Max(0f, factor);
    }
}

