using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BGMPlayer : MonoBehaviour
{
    public static BGMPlayer Instance;
    public AudioEventChannel bgmEventChannel;
    [Header("Audio Source & Mixer")]
    public AudioSource audioSource;
    public AudioMixerGroup bgmMixerGroup;

    [Header("Fade")]
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (audioSource.isPlaying && audioSource.clip == clip)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeIn(clip, volume));
    }

    public void StopBGM()
    {
        if (!audioSource.isPlaying)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn(AudioClip newClip, float targetVolume)
    {
        audioSource.clip = newClip;
        audioSource.outputAudioMixerGroup = bgmMixerGroup;
        audioSource.volume = 0f;
        audioSource.loop = true;
        audioSource.Play();

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }
        audioSource.Stop();
    }
    private void OnEnable()
    {
        if (bgmEventChannel != null)
        {
           // bgmEventChannel.OnPlayBGM += PlayBGM;
            //bgmEventChannel.OnStopBGM += StopBGM;
        }
    }
}
