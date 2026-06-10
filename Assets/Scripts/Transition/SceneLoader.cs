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
    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public GameSceneSO firstLoadScene;

    [Header("广播")]
    public FadeEventSO fadeEvent;
    
    
    [SerializeField] private GameSceneSO currentLoadedScene;
    
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    
    private bool fadeScreen;
    private bool isLoading;
    public float fadeDuration;
    
    private void Awake()
    {

        // Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        // currentLoadedScene = firstLoadScene;
        // currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
    }

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

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;
        GameFlowCoordinator.Instance.EnterFlow(GameFlowState.Loading);
        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;
        
        
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            fadeEvent.FadeIn(fadeDuration);
        }
        
        yield return new WaitForSecondsRealtime(fadeDuration);

        yield return currentLoadedScene.sceneReference.UnLoadScene();

        
        LoadNewScene();
    }
    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = sceneToLoad;
        SceneManager.SetActiveScene(obj.Result.Scene);
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
        isLoading = false;
    }
    
    
}
