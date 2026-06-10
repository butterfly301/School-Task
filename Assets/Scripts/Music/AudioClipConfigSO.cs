using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Clip Config")]
public class AudioClipConfigSO : ScriptableObject
{
    [System.Serializable]
    public class SoundEntry
    {
        public SoundEvent soundEvent;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public List<SoundEntry> entries = new List<SoundEntry>();

    private Dictionary<SoundEvent, SoundEntry> lookup;

    private void OnEnable()
    {
        lookup = new Dictionary<SoundEvent, SoundEntry>();
        foreach (var entry in entries)
        {
            if (!lookup.ContainsKey(entry.soundEvent))
                lookup.Add(entry.soundEvent, entry);
        }
    }

    public SoundEntry GetClip(SoundEvent evt)
    {
        if (lookup == null || lookup.Count == 0)
            OnEnable(); // 确保初始化
        return lookup.TryGetValue(evt, out var entry) ? entry : null;
    }
}
