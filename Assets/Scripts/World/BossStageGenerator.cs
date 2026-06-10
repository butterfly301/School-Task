using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
public class BossStageGenerator : MonoBehaviour
{
    public GameObject[] weedPrefabs;//杂草模型
    private string weedFolderPath = "Prefabs/Grass"; // 道具预制体的路径
    public GameObject[] flowerPrefabs;//花朵模型
    private string flowersFolderPath = "Prefabs/Flowers";
    public GameObject[] crackPrefabs;
    private string cracksFolderPath = "Prefabs/Cracks";

    public int crackPerRoad=1;
    public int weedDensity = 1;//杂草生成密度
    public int weedPerBlock = 40;
    public int flowerDensity = 1; //花朵生成密度
    public int flowerPerBlock = 10;
    public float minScale;
    public float maxScale;
    public float MapSize;
    
    private NavMeshSurface surface;
    private LayerMask collisionMask;

    public int[,] roadGrid = new int[3, 3]
    {
        {1,1,1},
        {1,0,1},
        {1,1,1}
    };

    void OnEnable()
    {
        surface = GetComponent<NavMeshSurface>();
        weedPrefabs = Resources.LoadAll<GameObject>(weedFolderPath);
        flowerPrefabs=Resources.LoadAll<GameObject>(flowersFolderPath);
        crackPrefabs=Resources.LoadAll<GameObject>(cracksFolderPath);
        GenerateNavMesh();
        GenerateWeeds(roadGrid);
        GenerateFlowers(roadGrid);
        GenerateCrack(roadGrid);
        //enerateEnemyPoint(roadGrid);
    }

    public Vector3 GetModelSize(GameObject model)
    {
        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }
        else
        {
            Debug.LogWarning("模型没有 Renderer 组件");
            return Vector3.zero;
        }
    }

    bool IsOverlapping(Vector3 position, Vector3 buildingSize, GameObject[] roads, Vector3 roadSize)
    {
        foreach (var road in roads)
        {
            Vector3 roadPosition = road.transform.position;
            Vector3 roadBounds = roadSize / 2; // 道路的半个尺寸

            // 检查楼房是否与道路重叠
            if (Mathf.Abs(position.x - roadPosition.x) < (buildingSize.x + roadBounds.x) &&
                Mathf.Abs(position.z - roadPosition.z) < (buildingSize.z + roadBounds.z))
            {
                return true; // 重叠
            }
        }
        return false; // 不重叠
    }

    void GenerateWeeds(int[,] roadGrid)
    {
        int width = roadGrid.GetLength(0);
        int height = roadGrid.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] == 0|| roadGrid[x, y] == 1)
                {
                    for (int i = 0; i < weedPerBlock; i++)
                    {
                        // 根据杂草生成密度随机生成杂草
                        if (Random.value < weedDensity)
                        {
                            // 随机选择一个杂草预制体
                            GameObject weedPrefab = weedPrefabs[Random.Range(0, weedPrefabs.Length)];

                            // 随机生成杂草的位置
                            Vector3 weedPosition = new Vector3(
                                Random.Range(40, 130),
                                0,
                                Random.Range(40, 130)
                            );

                            // 随机生成杂草的朝向
                            Quaternion weedRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                            // 生成杂草
                            GameObject weed = Instantiate(weedPrefab, weedPosition, weedRotation);
                            float randomScale = Random.Range(minScale, maxScale);
                            weed.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                        }
                    }
                }
            }
        }
    }
    
    void GenerateFlowers(int[,] roadGrid)
    {
        int width = roadGrid.GetLength(0);
        int height = roadGrid.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] == 0|| roadGrid[x, y] == 1)
                {
                    for (int i = 0; i < flowerPerBlock; i++)
                    {
                        // 根据杂草生成密度随机生成杂草
                        if (Random.value < flowerDensity)
                        {
                            // 随机选择一个杂草预制体
                            GameObject flowerPrefab = flowerPrefabs[Random.Range(0, flowerPrefabs.Length)];

                            // 随机生成杂草的位置
                            Vector3 flowerPosition = new Vector3(
                                Random.Range(40, 130),
                                0,
                                Random.Range(40, 130)
                            );

                            // 随机生成杂草的朝向
                            Quaternion flowerRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                            // 生成杂草
                            GameObject flower = Instantiate(flowerPrefab, flowerPosition, flowerRotation);
                            float randomScale = Random.Range(minScale, maxScale);
                            flower.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
                        }
                    }
                }
            }
        }
    }
    float roadWidth = 20; // 道路的宽度
    float roadLength = 20; // 道路的长度
    
    void GenerateCrack(int[,] roadGrid)
    {
        int width = roadGrid.GetLength(0);
        int height = roadGrid.GetLength(1);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] == 1)
                {
                    for (int i = 0; i < crackPerRoad; i++)
                    {
                        // 随机选择一个杂草预制体
                        GameObject crackSprite = crackPrefabs[Random.Range(0, crackPrefabs.Length)];
                        // 随机生成杂草的位置
                        Vector3 crackPosition = new Vector3(
                            x * roadWidth + Random.Range(-roadWidth / 2f, roadWidth / 2f),
                            0.4f,
                            y * roadLength + Random.Range(-roadLength / 2f, roadLength / 2f)
                        );

                        // 随机生成杂草的朝向
                        Quaternion crackRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        // 生成杂草
                        GameObject crack = Instantiate(crackSprite, crackPosition, crackRotation);
                        crack.transform.localScale = new Vector3(Random.Range(2f, 3f), Random.Range(2f, 3f), Random.Range(2f, 3f));
                    }
                }
            }
        }
    }
    
    public void GenerateNavMesh()
    {
        if (surface != null)
        {
            surface.BuildNavMesh(); // 动态烘焙NavMesh
        }
    }
    
    /*public List<GameObject> enemyPoints = new List<GameObject>();
    void GenerateEnemyPoint(int[,] roadGrid)
    {
        int width = roadGrid.GetLength(0);
        int height = roadGrid.GetLength(1);
        
        Vector3 roadSize = GetModelSize(straightRoadPrefab); // 获取道路模型的尺寸
        
        float roadWidth = roadSize.x; // 道路的宽度
        float roadLength = roadSize.z; // 道路的长度

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                    Vector3 enemyPointPosition = new Vector3(
                        x * roadWidth + Random.Range(-roadWidth / 2f, roadWidth / 2f),
                        0,
                        y * roadLength + Random.Range(-roadLength / 2f, roadLength / 2f));

                    // 检测该点是否与模型发生碰撞
                    if (!Physics.CheckSphere(enemyPointPosition, 0.1f, collisionMask))
                    {
                        // 如果没有碰撞，生成一个可视化对象来表示导航点
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        GameObject enemyPoint = Instantiate(obj, enemyPointPosition, Quaternion.identity);
                        enemyPoint.name = x + "_" + y + "_";
                        enemyPoint.transform.position = enemyPointPosition;
                        enemyPoints.Add(enemyPoint);
                }
            }
        }
    }*/
}
