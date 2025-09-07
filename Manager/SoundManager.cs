using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 효과음 데이터 저장용
    [System.Serializable]
    public class SoundEffectData
    {
        public string name;
        public AudioClip clip;
    }


    public static SoundManager Instance { get; private set; }

    [SerializeField] private SoundEffectData[] soundEffects;
    [SerializeField] private int poolSize = 10;

    private Dictionary<string, AudioClip> soundDict;
    private List<AudioSource> audioSourcePool;
    private Transform poolContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSoundDict();
        InitializeAudioSourcePool();
    }

    private void InitializeSoundDict()
    {
        soundDict = new Dictionary<string, AudioClip>();
        foreach (var s in soundEffects)
        {
            if (!soundDict.ContainsKey(s.name))
                soundDict.Add(s.name, s.clip);
        }
    }

    private void InitializeAudioSourcePool()
    {
        audioSourcePool = new List<AudioSource>();
        poolContainer = new GameObject("SFX_Pool").transform;
        poolContainer.parent = this.transform;

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = poolContainer.gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            audioSourcePool.Add(src);
        }
    }

    public void Play(string soundName, float volume = 1f, float pitch = 1f)
    {
        if (!soundDict.ContainsKey(soundName))
        {
            Debug.LogWarning($"Sound '{soundName}' not found.");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        if (source != null)
        {
            source.clip = soundDict[soundName];
            source.volume = volume;
            source.pitch = pitch;
            source.Play();
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var src in audioSourcePool)
        {
            if (!src.isPlaying)
                return src;
        }
        // 풀을 다 썼다면 하나를 재활용 (선택적)
        return audioSourcePool[0];
    }
}
