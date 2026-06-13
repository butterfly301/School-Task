using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Transform player;
    public Vector3 firstPosition;
    [Header("Event")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;

    [Header("Broadcast")]
    public FadeEventSO fadeEvent;

    [SerializeField] private GameSceneSO currentLoadedScene;

    private AsyncOperationHandle<SceneInstance>? currentSceneHandle;
    private string currentSceneAddress;
    private SceneFlowType currentFlowType = SceneFlowType.Loading;

    private GameSceneSO sceneToLoad;
    private string sceneAddressToLoad;
    private SceneFlowType flowTypeToLoad;
    private Vector3 positionToGo;

    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;

    private void Start()
    {
        GameFlowCoordinator.Instance.EnterFlow(GameFlowState.Loading);
        NewGame();
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
    }

    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        OnLoadRequestEvent(sceneToLoad, firstPosition, true);
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool shouldFadeScreen)
    {
        if (isLoading)
            return;

        GameFlowCoordinator.Instance.EnterFlow(GameFlowState.Loading);
        isLoading = true;
        sceneToLoad = locationToLoad;
        sceneAddressToLoad = locationToLoad.sceneReference.RuntimeKey.ToString();
        flowTypeToLoad = locationToLoad.flowType;
        positionToGo = posToGo;
        fadeScreen = shouldFadeScreen;

        if (currentSceneHandle.HasValue)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    public void LoadSavedScene(string sceneAddress, SceneFlowType flowType, Vector3 posToGo, bool shouldFadeScreen)
    {
        if (isLoading || string.IsNullOrEmpty(sceneAddress))
            return;

        GameFlowCoordinator.Instance.EnterFlow(GameFlowState.Loading);
        isLoading = true;
        sceneToLoad = null;
        sceneAddressToLoad = sceneAddress;
        flowTypeToLoad = flowType;
        positionToGo = posToGo;
        fadeScreen = shouldFadeScreen;

        if (currentSceneHandle.HasValue)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (fadeScreen)
        {
            fadeEvent.FadeIn(fadeDuration);
        }

        yield return new WaitForSecondsRealtime(fadeDuration);

        if (currentSceneHandle.HasValue)
        {
            yield return Addressables.UnloadSceneAsync(currentSceneHandle.Value);
            currentSceneHandle = null;
        }

        LoadNewScene();
    }

    private void LoadNewScene()
    {
        AsyncOperationHandle<SceneInstance> loadingOption = sceneToLoad != null
            ? sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true)
            : Addressables.LoadSceneAsync(sceneAddressToLoad, LoadSceneMode.Additive, true);

        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        currentLoadedScene = sceneToLoad;
        currentSceneHandle = operationHandle;
        currentFlowType = flowTypeToLoad;

        SceneManager.SetActiveScene(operationHandle.Result.Scene);
        currentSceneAddress = operationHandle.Result.Scene.path;
        if (player != null)
        {
            player.position = positionToGo;
        }

        if (fadeScreen)
        {
            fadeEvent.FadeOut(fadeDuration);
        }

        if (currentLoadedScene != null)
        {
            GameFlowCoordinator.Instance.EnterSceneFlow(currentLoadedScene.flowType);
        }
        else
        {
            GameFlowCoordinator.Instance.EnterSceneFlow(currentFlowType);
        }

        if (SaveManager.Instance != null && currentFlowType == SceneFlowType.Gameplay &&
            !SaveManager.Instance.IsRestoringSceneLoad)
        {
            SaveManager.Instance.SaveCheckpoint(currentSceneAddress, currentFlowType, positionToGo);
        }

        isLoading = false;
    }
}
