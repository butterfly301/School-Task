using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioEventChannel channel;
    [SerializeField] private AudioClipConfigSO clipConfig;
    [SerializeField] private AudioPool audioPool;
    public AudioSource SfxPlyer;

    [Header("��Ƶ����")]
    public AudioMixer AudioMixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup bgmGroup;

    private void OnEnable()
    {
        channel.On3DEvent.AddListener(Play3D);
        channel.On2DEvent.AddListener(Play2D);
    }

    private void OnDisable()
    {
        channel.On3DEvent.RemoveListener(Play3D);
        channel.On2DEvent.RemoveListener(Play2D);
    }

    void Play3D(SoundEvent evt, Vector3 pos)
    {
        var data = clipConfig.GetClip(evt);
        if (data == null) return;
        audioPool.Play(evt, data.clip, pos, data.volume, 128, sfxGroup);
    }

    void Play2D(SoundEvent evt)
    {
        var data = clipConfig.GetClip(evt);
        if (data == null) return;

        // ����һ���ٵ� 2D ����λ�ã������������
        Vector3 camPos = Camera.main ? Camera.main.transform.position : Vector3.zero;
        audioPool.Play(evt, data.clip, camPos, data.volume, 128, sfxGroup);
    }
    public void StopAllAudio()
    {
        audioPool.StopAll();
    }
    private void Awake()
    {
        SfxPlyer=GetComponent<AudioSource>();
        
    }
    public void StopSound(SoundEvent evt)
    {
        audioPool.Stop(evt);
    }
    public void SetSFXVolun(float volun=-80f)
    {
        AudioMixer.SetFloat("SFXVolume", volun); 
    }
    IEnumerator SetVolun(float delay)
    {

        yield return new WaitForSeconds(delay); 
        SetSFXVolun();

    }
    public void MuteSFXAfterDelay(float delay)
    {
        StartCoroutine(SetVolun(delay));
    }


}

