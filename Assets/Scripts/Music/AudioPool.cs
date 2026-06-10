using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioPool : MonoBehaviour
{
    [Header("基本设置")]
    public GameObject prefab;
    public int initialSize = 10;
    public int maxPoolSize = 50;


    [Header("并发限制")]
    public int defaultMaxConcurrent = 5;
    public Dictionary<SoundEvent, int> maxConcurrent = new();

    private Dictionary<SoundEvent, int> currentlyPlaying = new();
    private Queue<AudioObject> pool = new Queue<AudioObject>();
    private AudioSource audioSource;
   
    private void Start()
    {
        for (int i = 0; i < initialSize; i++)
            CreateNew();
    }

    private AudioObject CreateNew()
    {
        GameObject obj = Instantiate(prefab, transform);
        var audioObj = obj.GetComponent<AudioObject>();
        obj.SetActive(false);
        pool.Enqueue(audioObj);
        return audioObj;
    }

    public void Play(SoundEvent evt, AudioClip clip, Vector3 pos, float volume, int priority = 128, AudioMixerGroup mixerGroup = null)
    {
        if (clip == null) return;

        if (!maxConcurrent.ContainsKey(evt))
            maxConcurrent[evt] = defaultMaxConcurrent;
        if (!currentlyPlaying.ContainsKey(evt))
            currentlyPlaying[evt] = 0;

        // 并发限制
        if (currentlyPlaying[evt] >= maxConcurrent[evt])
        {
            if (priority > 128) return; // 低优先级直接丢弃
            // 高优先级继续（可选）
        }

        if (pool.Count == 0 && transform.childCount < maxPoolSize)
            CreateNew();

        if (pool.Count == 0) return; // 完全耗尽池，丢弃

        var audioObj = pool.Dequeue();
        currentlyPlaying[evt]++;
        

        audioObj.Play(clip, pos, volume, () =>
        {
            if (audioObj == null)
            {
                Debug.LogError("audioObj 是 null！");
            }
            if (clip == null)
            {
                Debug.LogError("clip 是 null！");
            }
            if (mixerGroup == null)
            {
                Debug.LogWarning("mixerGroup 是 null！");
            }
            currentlyPlaying[evt]--;
            ReturnToPool(audioObj);
        }, mixerGroup, priority,evt);
        
    }

    private void ReturnToPool(AudioObject obj)
    {
        pool.Enqueue(obj);
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StopAll()
    {
        foreach (Transform child in transform)
        {
            AudioObject obj = child.GetComponent<AudioObject>();
            if (obj != null && obj.IsPlaying())//确保是正在播放的
            {
                obj.Stop(); // 让 AudioObject 自行处理停止播放逻辑
                ReturnToPool(obj); // 回收到池中
            }
        }

        currentlyPlaying.Clear(); // 清空计数，防止后续播放被错误阻止
    }
    public void Stop(SoundEvent evt)
    {
        foreach (Transform child in transform)
        {
            AudioObject obj = child.GetComponent<AudioObject>();
            if (obj != null && obj.IsPlaying() && obj.CurrentEvent == evt)
            {
                obj.Stop(); // 停止音效
                ReturnToPool(obj); // 回收
            }
        }

        if (currentlyPlaying.ContainsKey(evt))
            currentlyPlaying[evt] = 0; // 重置计数
    }

}

