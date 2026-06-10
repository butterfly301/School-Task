using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Audio/Event Channel")]
public class AudioEventChannel : ScriptableObject
{
    [System.Serializable]
    public class SoundEvent3D : UnityEvent<SoundEvent, Vector3> { }

    [System.Serializable]
    public class SoundEvent2D : UnityEvent<SoundEvent> { }

    public SoundEvent3D On3DEvent = new SoundEvent3D();
    public SoundEvent2D On2DEvent = new SoundEvent2D();

    public void Raise3D(SoundEvent evt, Vector3 position)
    {
        On3DEvent?.Invoke(evt, position);
    }

    public void Raise2D(SoundEvent evt)
    {
        On2DEvent?.Invoke(evt);
    }
    public class SoundEventBGM: UnityEvent<SoundEvent> { }
    public SoundEventBGM OnBGM = new SoundEventBGM();
    public  void RaiseBGM(SoundEvent evt)
    {
        OnBGM?.Invoke(evt);
    }
    /*
    public event Action<AudioClip, float> OnPlayBGM;
    public event Action OnStopBGM;

    public void RaisePlayBGM(AudioClip clip, float volume = 1f)
        => OnPlayBGM?.Invoke(clip, volume);

    public void RaiseStopBGM()
        => OnStopBGM?.Invoke();*/

}
