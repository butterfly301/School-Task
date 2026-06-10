using UnityEngine;

public class DropSystem : MonoBehaviour
{
    public DropTable table;
    private float startTime;

    void Start() => startTime = Time.time;

    public void Drop(Vector3 pos)
    {
        float elapsed = Time.time - startTime;
        var dropItem = table.GetDropItem(elapsed);

        if (dropItem != null)
            Instantiate(dropItem.prefab, pos, Quaternion.identity);
    }
}

