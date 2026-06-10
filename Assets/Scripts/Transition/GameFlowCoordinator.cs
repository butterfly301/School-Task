using UnityEngine;

public enum GameFlowState
{
    Initialization,
    Persistent,
    Video,
    Loading,
    Menu,
    Gameplay,
    Pause,
    Overlay,
    Summary
}

public class GameFlowCoordinator : MonoBehaviour
{
    private static GameFlowCoordinator instance;
    private GameFlowState previousInteractiveState = GameFlowState.Gameplay;

    public static GameFlowCoordinator Instance
    {
        get
        {
            EnsureInstance();
            return instance;
        }
    }

    public GameFlowState CurrentState { get; private set; } = GameFlowState.Initialization;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        EnsureInstance();
    }

    private static void EnsureInstance()
    {
        if (instance != null)
        {
            return;
        }

        instance = FindObjectOfType<GameFlowCoordinator>();
        if (instance != null)
        {
            return;
        }

        GameObject coordinatorObject = new GameObject("GameFlowCoordinator");
        instance = coordinatorObject.AddComponent<GameFlowCoordinator>();
        DontDestroyOnLoad(coordinatorObject);
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
        ApplyState(CurrentState);
    }

    public void EnterFlow(GameFlowState state)
    {
        if (state == GameFlowState.Gameplay || state == GameFlowState.Menu || state == GameFlowState.Video)
        {
            previousInteractiveState = state;
        }

        CurrentState = state;
        ApplyState(state);
    }

    public void EnterSceneFlow(SceneFlowType flowType)
    {
        switch (flowType)
        {
            case SceneFlowType.Persistent:
                EnterFlow(GameFlowState.Persistent);
                break;
            case SceneFlowType.Video:
                EnterFlow(GameFlowState.Video);
                break;
            case SceneFlowType.Menu:
                EnterFlow(GameFlowState.Menu);
                break;
            case SceneFlowType.Loading:
                EnterFlow(GameFlowState.Loading);
                break;
            case SceneFlowType.Gameplay:
                EnterFlow(GameFlowState.Gameplay);
                break;
            default:
                EnterFlow(GameFlowState.Gameplay);
                break;
        }
    }

    public void EnterPause()
    {
        if (CurrentState != GameFlowState.Summary)
        {
            previousInteractiveState = GameFlowState.Gameplay;
        }

        EnterFlow(GameFlowState.Pause);
    }

    public void EnterOverlay()
    {
        previousInteractiveState = GameFlowState.Gameplay;
        EnterFlow(GameFlowState.Overlay);
    }

    public void EnterSummary()
    {
        EnterFlow(GameFlowState.Summary);
    }

    public void ResumeInteractiveFlow()
    {
        EnterFlow(previousInteractiveState == GameFlowState.Menu ? GameFlowState.Menu : GameFlowState.Gameplay);
    }

    private static void ApplyState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.Gameplay:
                Time.timeScale = 1f;
                CursorStateController.Instance.SetMode(CursorMode.GameplayLocked);
                break;
            case GameFlowState.Pause:
            case GameFlowState.Overlay:
            case GameFlowState.Summary:
                Time.timeScale = 0f;
                CursorStateController.Instance.SetMode(CursorMode.Visible);
                break;
            case GameFlowState.Menu:
            case GameFlowState.Loading:
            case GameFlowState.Video:
            case GameFlowState.Persistent:
            case GameFlowState.Initialization:
            default:
                Time.timeScale = 1f;
                CursorStateController.Instance.SetMode(CursorMode.Visible);
                break;
        }
    }
}
