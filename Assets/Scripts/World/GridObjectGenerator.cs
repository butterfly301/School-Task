using System;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectGenerator : MonoBehaviour
{
    public GameObject fireflyPrefab; // 要生成的物体预制体
    private Vector3 startPosition = new Vector3(0, 0, 0);
    private Vector3 endPosition = new Vector3(200, 5, 200);
    private float spacing = 20f;
    private List<GameObject> fireflies = new List<GameObject>();

    private void OnEnable()
    {
        GenerateObjects();
    }

    private void OnDisable()
    {
        foreach (GameObject firefly in fireflies)
        {
            Destroy(firefly);
        }
        fireflies.Clear();
    }

    void GenerateObjects()
    { 
        // 计算X轴和Z轴上的物体数量
        int xCount = Mathf.FloorToInt((endPosition.x - startPosition.x) / spacing) + 1;
        int zCount = Mathf.FloorToInt((endPosition.z - startPosition.z) / spacing) + 1;

        // 生成物体网格
        for (int x = 0; x < xCount; x++)
        {
            for (int z = 0; z < zCount; z++)
            {
                Vector3 spawnPosition = new Vector3(
                    startPosition.x + x * spacing,
                    startPosition.y,
                    startPosition.z + z * spacing
                );

                GameObject firefly=Instantiate(fireflyPrefab, spawnPosition, Quaternion.identity);
                fireflies.Add(firefly);
            }
        }
    }
}