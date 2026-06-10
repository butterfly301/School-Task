using UnityEngine;
using System;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioObject : MonoBehaviour
{
    public SoundEvent CurrentEvent { get; private set; } // 添加字段
    private AudioSource audioSource;
    private Action onComplete;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, Vector3 position, float volume, Action onComplete = null, AudioMixerGroup mixerGroup = null, int priority = 128, SoundEvent? evt = null)
    {
        this.onComplete = onComplete;
        this.CurrentEvent = evt ?? SoundEvent.StopAllSounds;
        transform.position = position;
        gameObject.SetActive(true);

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.outputAudioMixerGroup = mixerGroup;
        audioSource.priority = priority;
        audioSource.spatialBlend = 1f; // 确保是 3D 音效
        audioSource.Play();

        Invoke(nameof(Finish), clip.length);
    }

    private void Finish()
    {
        onComplete?.Invoke();
        gameObject.SetActive(false);
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
    public void Stop()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            CancelInvoke(nameof(Finish)); // 防止延迟回调
            Finish(); // 手动回收
        }
    }
}
