using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    private static Sprite whiteSprite;

    [SerializeField] private float width = 1.1f;
    [SerializeField] private float height = 0.14f;
    [SerializeField] private float verticalPadding = 0.45f;

    private Transform barRoot;
    private Transform fillAnchor;
    private SpriteRenderer backgroundRenderer;
    private SpriteRenderer fillRenderer;

    private void Awake()
    {
        EnsureVisuals();
    }

    private void LateUpdate()
    {
        if (barRoot == null || Camera.main == null)
            return;

        barRoot.forward = Camera.main.transform.forward;
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        EnsureVisuals();

        float normalizedHealth = maxHealth <= 0 ? 0f : Mathf.Clamp01((float)currentHealth / maxHealth);
        fillAnchor.localScale = new Vector3(normalizedHealth, 1f, 1f);
        fillRenderer.color = Color.Lerp(new Color(0.85f, 0.15f, 0.15f), new Color(0.2f, 0.95f, 0.3f), normalizedHealth);
    }

    public void SetVisible(bool isVisible)
    {
        EnsureVisuals();
        barRoot.gameObject.SetActive(isVisible);
    }

    private void EnsureVisuals()
    {
        if (barRoot != null)
            return;

        if (whiteSprite == null)
        {
            whiteSprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f));
        }

        barRoot = new GameObject("HealthBar").transform;
        barRoot.SetParent(transform, false);
        barRoot.localPosition = new Vector3(0f, CalculateHeightOffset(), 0f);
        barRoot.localRotation = Quaternion.identity;

        GameObject backgroundObject = new GameObject("Background");
        backgroundObject.transform.SetParent(barRoot, false);
        backgroundRenderer = backgroundObject.AddComponent<SpriteRenderer>();
        backgroundRenderer.sprite = whiteSprite;
        backgroundRenderer.color = new Color(0f, 0f, 0f, 0.75f);
        backgroundRenderer.sortingOrder = 200;
        backgroundObject.transform.localScale = new Vector3(width, height, 1f);

        fillAnchor = new GameObject("FillAnchor").transform;
        fillAnchor.SetParent(barRoot, false);
        fillAnchor.localPosition = new Vector3(-width * 0.5f, 0f, -0.01f);
        fillAnchor.localRotation = Quaternion.identity;

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(fillAnchor, false);
        fillObject.transform.localPosition = new Vector3(width * 0.5f, 0f, 0f);
        fillRenderer = fillObject.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = whiteSprite;
        fillRenderer.color = new Color(0.2f, 0.95f, 0.3f);
        fillRenderer.sortingOrder = 201;
        fillObject.transform.localScale = new Vector3(width, height * 0.75f, 1f);
    }

    private float CalculateHeightOffset()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return 2f;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            if (!(renderers[i] is SpriteRenderer))
            {
                bounds.Encapsulate(renderers[i].bounds);
            }
        }

        return bounds.max.y - transform.position.y + verticalPadding;
    }
}
