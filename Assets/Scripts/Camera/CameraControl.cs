using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;
    public float duration = 2f;

    private void OnEnable()
    {
        if (cameraShakeEvent != null)
        {
            cameraShakeEvent.OnEventRaised += OnCameraShake;
        }
    }

    private void OnDisable()
    {
        if (cameraShakeEvent != null)
        {
            cameraShakeEvent.OnEventRaised -= OnCameraShake;
        }
    }

    private void OnCameraShake()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }
    }
}
