using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [Serializable]
    private class SaveData
    {
        public string sceneAddress;
        public int flowType;
        public SerializableVector3 playerPosition;
        public float timeRemaining;
        public int money;
        public int shield;
        public bool canRevive;
        public bool item09Armed;
        public float timeSurvived;
        public int totalCoins;
        public int totalShields;
        public float distanceTraveled;
        public bool isAlive;
        public int totalKills;
        public int totalAttacks;
        public List<SavedInventoryItem> inventoryItems = new List<SavedInventoryItem>();
    }

    [Serializable]
    private class SavedInventoryItem
    {
        public int itemId;
        public int amount;
        public int price;
    }

    [Serializable]
    private struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    private static SaveManager instance;
    private readonly Dictionary<int, ItemData> itemCatalog = new Dictionary<int, ItemData>();
    private SaveData currentSaveData;
    private SaveData pendingRestoreData;

    public static SaveManager Instance => instance;
    public bool IsRestoringSceneLoad { get; private set; }

    private static string SaveFilePath => Path.Combine(Application.persistentDataPath, "checkpoint-save.json");

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static void EnsureInstance()
    {
        if (instance != null)
            return;

        instance = FindObjectOfType<SaveManager>();
        if (instance != null)
            return;

        GameObject saveManagerObject = new GameObject("SaveManager");
        instance = saveManagerObject.AddComponent<SaveManager>();
        DontDestroyOnLoad(saveManagerObject);
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        currentSaveData = LoadSaveDataFromDisk();
        BuildItemCatalog();
    }

    public bool HasSaveData()
    {
        return LoadSaveDataFromDisk() != null;
    }

    public void PrepareForNewGame()
    {
        DeleteSave();
        pendingRestoreData = null;
        IsRestoringSceneLoad = false;

        GameStatsManager.Instance?.ResetStats();

        PlayerStateController playerStateController = FindObjectOfType<PlayerStateController>();
        if (playerStateController != null)
        {
            playerStateController.ResetPlayer();
        }

        PlayerCharacter playerCharacter = FindObjectOfType<PlayerCharacter>();
        if (playerCharacter != null)
        {
            playerCharacter.money = 0;
            playerCharacter.shield = 1;
            playerCharacter.CanRevive = false;
            playerCharacter.isAlive = true;
        }

        PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
        if (playerAttack != null)
        {
            playerAttack.IsItem09Armed = false;
        }

        CountdownTimer countdownTimer = FindObjectOfType<CountdownTimer>();
        if (countdownTimer != null)
        {
            countdownTimer.SetRemainingTime(countdownTimer.initialTime);
        }
    }

    public void DeleteSave()
    {
        currentSaveData = null;
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
    }

    public bool ContinueFromMenu()
    {
        SaveData saveData = LoadSaveDataFromDisk();
        if (saveData == null)
            return false;

        SceneLoader sceneLoader = FindObjectOfType<SceneLoader>();
        if (sceneLoader == null)
            return false;

        BuildItemCatalog();
        pendingRestoreData = saveData;
        IsRestoringSceneLoad = true;
        sceneLoader.LoadSavedScene(saveData.sceneAddress, (SceneFlowType)saveData.flowType,
            saveData.playerPosition.ToVector3(), true);
        StartCoroutine(RestoreAfterSceneLoad());
        return true;
    }

    public void SaveCheckpoint(string sceneAddress, SceneFlowType flowType, Vector3 checkpointPosition)
    {
        BuildItemCatalog();

        PlayerStateController playerStateController = FindObjectOfType<PlayerStateController>();
        PlayerCharacter playerCharacter = FindObjectOfType<PlayerCharacter>();
        PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
        CountdownTimer countdownTimer = FindObjectOfType<CountdownTimer>();
        GameStatsManager stats = GameStatsManager.Instance;

        if (playerStateController == null || playerCharacter == null || playerAttack == null || countdownTimer == null || stats == null)
            return;

        SaveData saveData = new SaveData
        {
            sceneAddress = sceneAddress,
            flowType = (int)flowType,
            playerPosition = new SerializableVector3(checkpointPosition),
            timeRemaining = countdownTimer.GetRemainingTime(),
            money = playerCharacter.money,
            shield = playerCharacter.shield,
            canRevive = playerCharacter.CanRevive,
            item09Armed = playerAttack.IsItem09Armed,
            timeSurvived = stats.timeSurvived,
            totalCoins = stats.totalCoins,
            totalShields = stats.totalShields,
            distanceTraveled = stats.distanceTraveled,
            isAlive = stats.isAlive,
            totalKills = stats.totalKills,
            totalAttacks = stats.totalAttacks,
            inventoryItems = SerializeInventory(playerStateController.inventory)
        };

        currentSaveData = saveData;
        File.WriteAllText(SaveFilePath, JsonUtility.ToJson(saveData, true));
    }

    public int GetPersistentItemCount(int itemId)
    {
        PlayerStateController playerStateController = FindObjectOfType<PlayerStateController>();
        if (playerStateController != null && playerStateController.inventory != null)
        {
            int liveCount = playerStateController.inventory.GetItemCount(itemId);
            if (liveCount > 0 || !IsRestoringSceneLoad)
                return liveCount;
        }

        SaveData saveData = pendingRestoreData ?? currentSaveData ?? LoadSaveDataFromDisk();
        if (saveData == null)
            return 0;

        return saveData.inventoryItems
            .Where(item => item.itemId == itemId)
            .Sum(item => item.amount);
    }

    public bool GetSavedCanRevive()
    {
        SaveData saveData = pendingRestoreData ?? currentSaveData;
        return saveData != null && saveData.canRevive;
    }

    public bool GetSavedItem09Armed()
    {
        SaveData saveData = pendingRestoreData ?? currentSaveData;
        return saveData != null && saveData.item09Armed;
    }

    private IEnumerator RestoreAfterSceneLoad()
    {
        while (pendingRestoreData == null || SceneManager.GetActiveScene().path != pendingRestoreData.sceneAddress)
        {
            yield return null;
        }

        yield return null;
        yield return null;

        RestoreRuntimeState(pendingRestoreData);
        currentSaveData = pendingRestoreData;
        pendingRestoreData = null;
        IsRestoringSceneLoad = false;
    }

    private void RestoreRuntimeState(SaveData saveData)
    {
        BuildItemCatalog();

        PlayerStateController playerStateController = FindObjectOfType<PlayerStateController>();
        PlayerCharacter playerCharacter = FindObjectOfType<PlayerCharacter>();
        PlayerAttack playerAttack = FindObjectOfType<PlayerAttack>();
        CountdownTimer countdownTimer = FindObjectOfType<CountdownTimer>();
        GameStatsManager stats = GameStatsManager.Instance;

        if (playerStateController == null || playerCharacter == null || playerAttack == null || countdownTimer == null || stats == null)
            return;

        List<ItemInstance> restoredItems = new List<ItemInstance>();
        foreach (SavedInventoryItem inventoryItem in saveData.inventoryItems)
        {
            if (!itemCatalog.TryGetValue(inventoryItem.itemId, out ItemData itemData) || itemData == null)
                continue;

            ItemInstance itemInstance = new ItemInstance(itemData)
            {
                currentAmount = inventoryItem.amount,
                currentPrice = inventoryItem.price
            };
            restoredItems.Add(itemInstance);
        }

        playerStateController.inventory.SetItems(restoredItems);
        playerCharacter.money = saveData.money;
        playerCharacter.shield = saveData.shield;
        playerCharacter.CanRevive = saveData.canRevive;
        playerCharacter.isAlive = saveData.isAlive;
        playerAttack.IsItem09Armed = saveData.item09Armed;
        countdownTimer.SetRemainingTime(saveData.timeRemaining);
        stats.ApplyStats(saveData.timeSurvived, saveData.totalCoins, saveData.totalShields,
            saveData.distanceTraveled, saveData.isAlive, saveData.totalKills, saveData.totalAttacks);

        if (playerStateController.transform != null)
        {
            playerStateController.transform.position = saveData.playerPosition.ToVector3();
        }
    }

    private List<SavedInventoryItem> SerializeInventory(Inventory inventory)
    {
        if (inventory == null)
            return new List<SavedInventoryItem>();

        return inventory.GetItemList()
            .Where(item => item?.baseData != null)
            .Select(item => new SavedInventoryItem
            {
                itemId = item.GetItemID(),
                amount = item.currentAmount,
                price = item.currentPrice
            })
            .ToList();
    }

    private SaveData LoadSaveDataFromDisk()
    {
        if (!File.Exists(SaveFilePath))
            return null;

        string json = File.ReadAllText(SaveFilePath);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonUtility.FromJson<SaveData>(json);
    }

    private void BuildItemCatalog()
    {
        if (itemCatalog.Count > 0)
            return;

        GameObject[] itemPrefabs = Resources.LoadAll<GameObject>("Prefabs/Items");
        foreach (GameObject itemPrefab in itemPrefabs)
        {
            Item item = itemPrefab.GetComponent<Item>();
            if (item != null && item.itemData != null)
            {
                itemCatalog[item.itemData.itemID] = item.itemData;
            }
        }
    }
}
