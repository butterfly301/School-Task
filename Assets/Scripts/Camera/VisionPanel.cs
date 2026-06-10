using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VisionPanel : MonoBehaviour
{
    private Camera camera;
    public GameObject glitchDamage;
    public GameObject shieldBreak;
    public GameObject interactiveTip;
    public GameObject priceTip;
    public GameObject warning;
    public GameObject timePriceTip;

    private void Start()
    {
        camera = Camera.main;
    }

    public void onGlitchDamageEnable()
    {
        glitchDamage.SetActive(true);
    }

    public void onGlitchDamageDisable()
    {
        glitchDamage.SetActive(false);
    }

    public void onShieldBreakEnable(Transform transform)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
        var rectTransform = shieldBreak.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
        shieldBreak.SetActive(true);
        StartCoroutine(onShieldBreakDisable());
    }

    IEnumerator onShieldBreakDisable()
    {
        yield return new WaitForSeconds(0.5f);
        shieldBreak.SetActive(false);
    }
    
    public void onInteractTipEnable(Transform transform)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
        var rectTransform = interactiveTip.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
        interactiveTip.SetActive(true);
    }

    public void onInteractTipKeep(Transform transform)
    {
        if (interactiveTip.activeSelf)
        {
            Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
            var rectTransform = interactiveTip.GetComponent<RectTransform>();
            rectTransform.position = screenPosition;
        }
    }

    public void onInteractTipDisable()
    {
        interactiveTip.SetActive(false);
    }

    public void onPriceTipEnable(Transform transform)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
        var rectTransform = priceTip.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
        priceTip.SetActive(true);
    }

    public void onPriceTipKeep(Transform transform)
    {
        if (priceTip.activeSelf)
        {
            Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
            var rectTransform = priceTip.GetComponent<RectTransform>();
            rectTransform.position = screenPosition;
        }
    }

    public void onPriceTipDisable()
    {
        priceTip.SetActive(false);
    }

    public void onWarningEnable(Transform transform)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
        var rectTransform = warning.GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
        warning.SetActive(true);
    }

    public void onWarningKeep(Transform transform)
    {
        if (warning.activeSelf)
        {
            Vector3 screenPosition = camera.WorldToScreenPoint(transform.position);
            var rectTransform = warning.GetComponent<RectTransform>();
            rectTransform.position = screenPosition;
        }
    }

    public void onWarningDisable()
    {
        warning.SetActive(false);
    }

    private Vector3 offset=new Vector3(0,2,0);
    public void onTimePriceTipEnable(Transform transform)
    {
        Vector3 screenPosition = camera.WorldToScreenPoint(transform.position+offset);
        var rectTransform = timePriceTip .GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
        timePriceTip.SetActive(true);
    }

    public void onTimePriceTipKeep(Transform transform)
    {
        if (timePriceTip.activeSelf)
        {
            Vector3 screenPosition = camera.WorldToScreenPoint(transform.position+offset);
            var rectTransform = timePriceTip.GetComponent<RectTransform>();
            rectTransform.position = screenPosition;
        }
    }

    public void onTimePriceTipDisable()
    {
        timePriceTip.SetActive(false);
    }
}
