using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CityGenerator : MonoBehaviour
{
    public GameObject straightRoadPrefab; // 直路
    public GameObject cornerRoadPrefab;   // 拐角
    public GameObject tJunctionRoadPrefab; // 三向路口
    public GameObject crossRoadPrefab;    // 十字路口
    
    public GameObject[] buildingPrefabs;  // 楼房模型
    private string buildingFolderPath = "Prefabs/Buildings"; // 道具预制体的路径
    public GameObject[] carPrefabs;       // 汽车模型
    private string carFolderPath = "Prefabs/Vehicles"; // 道具预制体的路径
    public GameObject[] propPrefabs;         // 小物件模型
    private string PropsFolderPath = "Prefabs/Props"; // 道具预制体的路径
    public GameObject[] weedPrefabs;//杂草模型
    private string weedFolderPath = "Prefabs/Grass"; // 道具预制体的路径
    public GameObject[] flowerPrefabs;//花朵模型
    private string flowersFolderPath = "Prefabs/Flowers";
    public GameObject[] crackPrefabs;
    private string cracksFolderPath = "Prefabs/Cracks";
    public GameObject[] mountainPrefabs;
    private string mountainsFolderPath = "Prefabs/Mountains";
    
    public float buildingSpacing = 25f;   // 楼房间隔
    public int carsPerRoad = 4;           // 每条道路上生成的石头数量
    public int crackPerRoad = 1;           // 每条道路上生成的汽车数量
    public float propDensity = 1f;      // 小物件生成密度（0 到 1）
    public float weedDensity = 1f;//杂草生成密度
    public float weedPerBlock = 40f;
    public float flowerDensity = 1f; //花朵生成密度
    public float flowerPerBlock = 10f;
    public float minScale;
    public float maxScale;
    public float MapSize;
    public int maxEnemies; // 最大敌人数
    // 在类变量区域添加以下变量
    public float initialSpawnInterval = 2f; // 初始生成间隔(秒)
    public float minSpawnInterval = 0.5f;   // 最小生成间隔
    public float spawnIntervalDecreaseRate = 0.05f; // 每10秒减少的生成间隔

    public float initialEnemySpeedMultiplier = 1f; // 初始敌人速度倍率
    public float maxEnemySpeedMultiplier = 2f;    // 最大敌人速度倍率
    public float speedIncreaseRate = 0.1f;      // 每10秒增加的速度倍率

    private float gameTime = 0f; // 游戏运行时间
    private float lastDifficultyUpdateTime = 0f; // 上次更新难度的时间
    private float difficultyUpdateInterval = 5f; // 难度更新间隔(秒)

    public List<GameObject> roads = new List<GameObject>(); // 存储生成的道路
    public List<GameObject> cars = new List<GameObject>();  // 存储生成的汽车
    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> spawnPoints = new List<GameObject>();
    
    private NavMeshSurface surface;
    
    public LayerMask collisionMask;
    
    private Coroutine enemySpawnCoroutine; // 用于控制敌人生成的协程

    /*public int[,] roadGrid = new int[10, 10]
    {
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1 ,1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 1, 0, 0, 0, 0, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
    };*/
    public int[,] roadGrid = new int[9, 9]
   {
       {1, 1, 1, 1, 1, 1, 1, 1, 1,},
       {1, 0, 0, 0, 1, 0, 0, 0, 1,},
       {1, 0, 0, 0, 1, 0, 0, 0, 1, },
       {1, 0, 0, 0, 1, 0, 0, 0, 1, },
       {1, 1, 1, 1, 1, 1, 1, 1, 1 ,},
       {1, 0, 0, 0, 1, 0, 0, 0, 1, },
       {1, 0, 0, 0, 1, 0, 0, 0, 1, },
       {1, 0, 0, 0, 1, 0, 0, 0, 1, },
       {1, 1, 1, 1, 1, 1, 1, 1, 1, },
   };

    void OnEnable()
    {
        surface = GetComponent<NavMeshSurface>();
        buildingPrefabs = Resources.LoadAll<GameObject>(buildingFolderPath);
        carPrefabs = Resources.LoadAll<GameObject>(carFolderPath);
        propPrefabs = Resources.LoadAll<GameObject>(PropsFolderPath);
        weedPrefabs = Resources.LoadAll<GameObject>(weedFolderPath);
        flowerPrefabs=Resources.LoadAll<GameObject>(flowersFolderPath);
        crackPrefabs=Resources.LoadAll<GameObject>(cracksFolderPath);
        mountainPrefabs=Resources.LoadAll<GameObject>(mountainsFolderPath);
        GenerateRoad(roadGrid);
        GenerateBuildings(roadGrid, roads.ToArray());
        GenerateCars(roadGrid);
        //GenerateProps(roadGrid);
        GenerateNavMesh();
        GenerateWeeds(roadGrid);
        GenerateFlowers(roadGrid);
        GenerateCrack(roadGrid);
        GenerateSpawnPoint(roadGrid);
        GenerateFacilities();
        GenerateEnemies();
        StartEnemySpawning();
    }

    void OnDisable()
    {
        StopEnemySpawning();
    }

    void GenerateRoad(int[,] roadGrid)
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
                if (roadGrid[x, y] == 1) // 如果是道路
                {
                    Vector3 position = new Vector3(x * roadWidth, 0, y * roadLength);
                    Quaternion rotation = Quaternion.identity;

                    // 检查相邻格子
                    bool hasRoadAbove = y < height - 1 && roadGrid[x, y + 1] == 1;
                    bool hasRoadBelow = y > 0 && roadGrid[x, y - 1] == 1;
                    bool hasRoadLeft = x > 0 && roadGrid[x - 1, y] == 1;
                    bool hasRoadRight = x < width - 1 && roadGrid[x + 1, y] == 1;

                    // 计算相邻道路数量
                    int roadCount = (hasRoadAbove ? 1 : 0) +
                                    (hasRoadBelow ? 1 : 0) +
                                    (hasRoadLeft ? 1 : 0) +
                                    (hasRoadRight ? 1 : 0);

                    // 根据相邻道路数量选择模型
                    GameObject roadPrefab = straightRoadPrefab;
                    if (roadCount == 2) // 双向拐角或直路
                    {
                        if ((hasRoadAbove && hasRoadBelow) || (hasRoadLeft && hasRoadRight))
                        {
                            roadPrefab = straightRoadPrefab; // 直路
                            if (hasRoadAbove && hasRoadBelow)
                                rotation = Quaternion.identity; // 水平道路
                            else if (hasRoadLeft && hasRoadRight)
                                rotation = Quaternion.Euler(0, 90, 0); // 垂直道路
                        }
                        else
                        {
                            roadPrefab = cornerRoadPrefab; // 拐角
                            if (hasRoadAbove && hasRoadRight)
                                rotation = Quaternion.Euler(0, 90, 0); // 右上拐角
                            else if (hasRoadRight && hasRoadBelow)
                                rotation = Quaternion.Euler(0, 180, 0); // 右下拐角
                            else if (hasRoadBelow && hasRoadLeft)
                                rotation = Quaternion.Euler(0, -90, 0); // 左下拐角
                            else if (hasRoadLeft && hasRoadAbove)
                                rotation = Quaternion.Euler(0, 0, 0); // 左上拐角
                        }
                    }
                    else if (roadCount == 3) // 三向路口
                    {
                        roadPrefab = tJunctionRoadPrefab;
                        if (!hasRoadBelow)
                            rotation = Quaternion.Euler(0, 0, 0); // 上、左、右
                        else if (!hasRoadLeft)
                            rotation = Quaternion.Euler(0, 90, 0); // 上、右、下
                        else if (!hasRoadAbove)
                            rotation = Quaternion.Euler(0, 180, 0); // 右、下、左
                        else if (!hasRoadRight)
                            rotation = Quaternion.Euler(0, 270, 0); // 下、左、上
                    }
                    else if (roadCount == 4) // 十字路口
                    {
                        roadPrefab = crossRoadPrefab;
                    }

                    GameObject road = Instantiate(roadPrefab, position, rotation);
                    road.isStatic = true;
                    foreach (var r in road.GetComponentsInChildren<Renderer>())
                    {
                        r.forceRenderingOff = true;
                        r.gameObject.AddComponent<NavMeshModifier>().overrideArea = true;
                    }
                    road.name = x.ToString()+","+y.ToString();
                    roads.Add(road); // 将生成的道路添加到列表中
                }
            }
        }
    }
    void GenerateBuildings(int[,] roadGrid, GameObject[] roads)
    {
        int width = roadGrid.GetLength(0);
        int height = roadGrid.GetLength(1);

        Vector3 buildingSize = GetModelSize(buildingPrefabs[0]); // 获取楼房模型的尺寸
        Vector3 roadSize = GetModelSize(straightRoadPrefab); // 获取道路模型的尺寸

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roadGrid[x, y] == 0) // 如果是可建区域
                {
                    Vector3 position = new Vector3(x * buildingSpacing, 0, y * buildingSpacing);

                    // 检查是否与道路重叠
                    if (!IsOverlapping(position, buildingSize, roads, roadSize))
                    {
                        for (int n = 0; n < 2; n++)
                        {
                            Vector3 pos = position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
                            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
                            GameObject building = Instantiate(buildingPrefabs[Random.Range(0, buildingPrefabs.Length)], pos, rotation);
                            building.name = x.ToString()+","+y.ToString()+"_"+n;
                            building.isStatic = true;
                            buildings.Add(building);
                        }
                    }
                }
            }
        }
    }
    
   void GenerateCars(int[,] roadGrid)
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
                if (roadGrid[x, y] == 1) // 如果是道路
                {
                    if (x != 4 && y != 4)
                    {
                        // 在每条道路上生成指定数量的汽车
                        for (int i = 0; i < carsPerRoad; i++)
                        {
                            // 随机选择一个汽车模型
                            GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

                            // 生成汽车的位置和朝向
                            Vector3 carPosition;
                            Quaternion carRotation;
                            bool isColliding;
                            int attempts = 0; // 防止无限循环

                            do
                            {
                                // 随机生成汽车的位置
                                carPosition = new Vector3(
                                    x * roadWidth + Random.Range(-roadWidth / 2f, roadWidth / 2f),
                                    0.2f,
                                    y * roadLength + Random.Range(-roadLength / 2f, roadLength / 2f)
                                );

                                // 随机生成汽车的朝向
                                carRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                                // 检测石头是否与其他石头碰撞
                                Vector3 carSize = GetModelSize(carPrefab);
                                isColliding = false;
                                foreach (var car in cars)
                                {
                                    if (Vector3.Distance(carPosition, car.transform.position) < carSize.magnitude)
                                    {
                                        isColliding = true;
                                        break;
                                    }
                                }

                                attempts++;
                                if (attempts > 10) // 防止无限循环
                                {
                                    break;
                                }
                            } while (isColliding);

                            // 生成石头
                            if (!isColliding)
                            {
                                carPosition.y = 0;
                                carRotation = Random.rotation;
                                GameObject car = Instantiate(carPrefab, carPosition, carRotation);
                                float rockScale = Random.Range(600f, 700f);
                                car.transform.localScale = new Vector3(rockScale, rockScale, rockScale);
                                car.isStatic = true;
                                cars.Add(car); // 将生成的石头添加到列表中
                            }
                        }
                    }
                }
            }
        }
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
            Debug.Log(model.name);
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
    


   

   /* void GenerateProps(int[,] roadGrid)
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
                if (roadGrid[x, y] == 0) // 如果是可建区域
                {
                    // 根据杂草生成密度随机生成杂草
                    if (Random.value < propDensity)
                    {
                        // 随机选择一个杂草预制体
                        GameObject propPrefab = propPrefabs[Random.Range(0, propPrefabs.Length)];

                        // 随机生成杂草的位置
                        Vector3 propPosition = new Vector3(
                            x * roadWidth + Random.Range(-roadWidth / 2f, roadWidth / 2f),
                            0,
                            y * roadLength + Random.Range(-roadLength / 2f, roadLength / 2f)
                        );

                        // 随机生成杂草的朝向
                        Quaternion propRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        // 生成杂草
                        GameObject prop = Instantiate(propPrefab, propPosition, propRotation);
                        prop.isStatic = true;
                    }
                }
            }
        }
    }*/

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
                                Random.Range(0, MapSize),
                                0,
                                Random.Range(0, MapSize)
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
                                Random.Range(0, MapSize),
                                0,
                                Random.Range(0, MapSize)
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
                            Vector3 roadSize = GetModelSize(straightRoadPrefab); // 获取道路模型的尺寸
                            float roadWidth = roadSize.x; // 道路的宽度
                            float roadLength = roadSize.z; // 道路的长度
                            // 随机生成杂草的位置
                            Vector3 crackPosition = new Vector3(
                                x * roadWidth + Random.Range(-roadWidth / 2f, roadWidth / 2f),
                                0.3f,
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
    
    
    void GenerateSpawnPoint(int[,] roadGrid)
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
                    Vector3 spawnPointPosition = new Vector3(
                        x * roadWidth + Random.Range(-roadWidth / 2f, roadWidth / 2f),
                        0,
                        y * roadLength + Random.Range(-roadLength / 2f, roadLength / 2f));

                    // 检测该点是否与模型发生碰撞
                    if (!Physics.CheckSphere(spawnPointPosition, 1f, collisionMask))
                    {
                        // 如果没有碰撞，生成一个可视化对象来表示导航点
                        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        GameObject spawnPoint = Instantiate(obj, spawnPointPosition, Quaternion.identity);
                        Destroy(obj);
                        spawnPoint.name = x + "_" + y;
                        spawnPoint.transform.position = spawnPointPosition;
                        spawnPoint.transform.localScale = new Vector3(0, 0, 0);
                        spawnPoints.Add(spawnPoint);
                    }
            }
        }
    }

    /*void SpawnEnemy()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("没有可用的生成点！");
            return;
        }

        for (int i = 0; i < maxEnemies; i++)
        {
            // 从对象池获取各种敌人
            SpawnEnemyType("CommonEnemy", i);
            SpawnEnemyType("ShieldEnemy", i);
            SpawnEnemyType("SlowEnemy", i);
            SpawnEnemyType("ExplodeEnemy", i);
            
            SpawnEnemyType("BulletEnemy", i);
        }
    }*/

    void SpawnEnemyType(string enemyType, int index)
    {
        if (spawnPoints.Count == 0) return;

        GameObject enemy = MyPooler.ObjectPooler.Instance.GetFromPool(
            enemyType,
            GetRandomPosition(),
            Quaternion.identity);
    
        enemy.name = enemyType + index.ToString();
    
        var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        var enemyAI = enemy.GetComponent<EnemyAI>();
    
        if (agent == null)
        {
            agent = enemy.AddComponent<UnityEngine.AI.NavMeshAgent>();
        }
    
        // 应用当前速度倍率
        if (enemyAI != null)
        {
            agent.speed = enemyAI.baseSpeed * initialEnemySpeedMultiplier;
        }
    }

    Vector3 GetRandomPosition()
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("没有可用的生成点！返回默认位置");
            return Vector3.zero;
        }

        // 随机选择一个生成点位置
        int randomIndex = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomIndex].transform.position;
    }
    
    // 开始敌人生成协程
    void StartEnemySpawning()
    {
        if (enemySpawnCoroutine != null) StopCoroutine(enemySpawnCoroutine);
        enemySpawnCoroutine = StartCoroutine(SpawnEnemiesRoutine());
    }

// 停止敌人生成协程
    void StopEnemySpawning()
    {
        if (enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
            enemySpawnCoroutine = null;
        }
    }

// 每2秒生成一个敌人的协程
    IEnumerator SpawnEnemiesRoutine()
    {
        float currentSpawnInterval = initialSpawnInterval;
    
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnInterval);
        
            // 更新游戏时间和难度
            gameTime += currentSpawnInterval;
            if (gameTime - lastDifficultyUpdateTime >= difficultyUpdateInterval)
            {
                UpdateDifficulty();
                lastDifficultyUpdateTime = gameTime;
            }
        
            // 如果当前敌人数未达上限，生成一个随机类型敌人
            if (CountActiveEnemies() < maxEnemies)
            {
                string[] enemyTypes = { "CommonEnemy", "ShieldEnemy", "SlowEnemy", "ExplodeEnemy", "BulletEnemy" };
                string randomType = enemyTypes[Random.Range(0, enemyTypes.Length)];
                SpawnEnemyType(randomType, Random.Range(0, 1000));
            }
        }
    }
    
    
// 添加新的难度更新方法
    void UpdateDifficulty()
    {
        // 减少生成间隔(提高生成频率)
        float newSpawnInterval = initialSpawnInterval - (spawnIntervalDecreaseRate * (gameTime / difficultyUpdateInterval));
        initialSpawnInterval = Mathf.Max(newSpawnInterval, minSpawnInterval);
    
        // 增加敌人速度
        float newSpeedMultiplier = initialEnemySpeedMultiplier + (speedIncreaseRate * (gameTime / difficultyUpdateInterval));
        initialEnemySpeedMultiplier = Mathf.Min(newSpeedMultiplier, maxEnemySpeedMultiplier);
    
        // 更新现有敌人的速度
        UpdateExistingEnemiesSpeed();
    }

// 更新现有敌人的速度
    void UpdateExistingEnemiesSpeed()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
            var enemyAI = enemy.GetComponent<EnemyAI>();
        
            if (agent != null && enemyAI != null)
            {
                agent.speed = enemyAI.baseSpeed * initialEnemySpeedMultiplier;
            }
        }
    }

// 统计当前活跃敌人数
    int CountActiveEnemies()
    {
        // 假设敌人都有"Enemy"标签
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    
    public GameObject[] facilityPrefab;
    public GameObject portalPrefab;  // 传送门预制体

    public GameSceneSO sceneToGo;
    // public GameObject chestPrefab;           //宝箱预制体
    // public GameObject explosiveBarrelPrefab; // 炸药桶预制体
    // public GameObject altarPrefab;           // 祭坛预制体
    // public GameObject stripPrefab;           // 钉刺带预制体
    //public GameObject turretPrefab;          //炮台预制体
    
    [Tooltip("每种设施(除传送门外)的生成数量")]
    public int[] facilityCountPerType;     // 每种设施的生成数量
    public List<GameObject> allFacilities = new List<GameObject>(); // 存储所有生成的设施
    
    void GenerateFacilities()
    {
        int facilityTotalCount=0;
        for (int i = 0; i < facilityCountPerType.Length; i++)
            facilityTotalCount += facilityCountPerType[i];
        // 确保有足够的生成点
        if (spawnPoints.Count < facilityTotalCount) // 3种设施各facilityCountPerType个
        {
            Debug.LogWarning("Not enough spawn points for all facilities!");
            return;
        }

        // 复制spawnPoints列表以便我们可以从中移除已使用的点
        List<GameObject> availableSpawnPoints = new List<GameObject>(spawnPoints);
        
        for(int i = 0; i < facilityPrefab.Length; i++)
            GenerateFacilityType(facilityPrefab[i], facilityCountPerType[i],ref availableSpawnPoints);
        
        // 生成传送门（从剩余的生成点中随机选一个）
        if (portalPrefab != null && availableSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            GameObject portal = Instantiate(
                portalPrefab, 
                availableSpawnPoints[randomIndex].transform.position, 
                Quaternion.identity
            );
            var teleporter = portal.GetComponent<Teleporter>();
            if (teleporter != null)
                teleporter.sceneToGo = sceneToGo;
            allFacilities.Add(portal); // 将传送门加入设施列表
        }
        else
        {
            Debug.LogWarning("未分配传送门预制体，或没有可用的生成点！");
        }
    }

    void GenerateFacilityType(GameObject prefab, int count, ref List<GameObject> spawnPoints)
    {
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            if (spawnPoints.Count == 0) break;

            int randomIndex = Random.Range(0, spawnPoints.Count);
            GameObject facility = Instantiate(prefab, 
                                             spawnPoints[randomIndex].transform.position, 
                                             Quaternion.identity);
            allFacilities.Add(facility);
            spawnPoints.RemoveAt(randomIndex); // 移除已使用的生成点
        }
    }

    // 清除所有生成的设施(如果需要重新生成)
    public void ClearAllFacilities()
    {
        foreach (var facility in allFacilities)
        {
            if (facility != null)
            {
                Destroy(facility);
            }
        }
        allFacilities.Clear();
    }

    // 重新生成所有设施
    public void RegenerateFacilities()
    {
        ClearAllFacilities();
        GenerateFacilities();
    }

    void GenerateEnemies()
    {
        gameTime = 0;
        for (int i = 0; i < 20; i++)
        {
            string[] enemyTypes = { "CommonEnemy", "ShieldEnemy", "SlowEnemy", "ExplodeEnemy", "BulletEnemy" };
            string randomType = enemyTypes[Random.Range(0, enemyTypes.Length)];
            SpawnEnemyType(randomType, i);
        }
    }
}