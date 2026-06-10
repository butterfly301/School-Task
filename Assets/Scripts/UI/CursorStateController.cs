using UnityEngine;

public enum CursorMode
{
    GameplayLocked,
    Visible
}

public class CursorStateController : MonoBehaviour
{
    private static CursorStateController instance;

    public static CursorStateController Instance
    {
        get
        {
            EnsureInstance();
            return instance;
        }
    }

    public CursorMode CurrentMode { get; private set; } = CursorMode.Visible;

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

        instance = FindObjectOfType<CursorStateController>();
        if (instance != null)
        {
            return;
        }

        GameObject controllerObject = new GameObject("CursorStateController");
        instance = controllerObject.AddComponent<CursorStateController>();
        DontDestroyOnLoad(controllerObject);
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
        Apply(CurrentMode);
    }

    public void SetMode(CursorMode mode)
    {
        CurrentMode = mode;
        Apply(mode);
    }

    private static void Apply(CursorMode mode)
    {
        if (mode == CursorMode.GameplayLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
