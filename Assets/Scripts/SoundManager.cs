using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager I { get; private set; }

    [Header("Main Sources")]
    public AudioSource bgmSource;   // untuk BGM (loop)
    public AudioSource sfxSource;   // untuk SFX (opsional)

    [Header("BGM Map per Scene")]
    public List<SceneBgmEntry> sceneBgms = new(); // isi di Inspector

    [Header("Default / Fallback")]
    public AudioClip defaultBgm;
    [Range(0f, 1f)] public float bgmVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public float fadeTime = 0.75f;

    Dictionary<string, AudioClip> map;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // siapkan dictionary & volume
        map = new Dictionary<string, AudioClip>();
        foreach (var e in sceneBgms)
        {
            if (e != null && e.clip != null && !string.IsNullOrEmpty(e.sceneName))
                map[e.sceneName] = e.clip;
        }

        if (bgmSource != null) { bgmSource.loop = true; bgmSource.volume = bgmVolume; }
        if (sfxSource != null) sfxSource.volume = sfxVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Start()  // jika mainmenu adalah scene pertama
    {
        // mainkan BGM pertama berdasar scene saat ini
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // cari BGM berdasar nama scene
        if (!map.TryGetValue(scene.name, out var clip))
            clip = defaultBgm;

        if (clip != null) PlayBGM(clip);
        else StopBGM();
    }

    // === API ===
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;
        StopAllCoroutines();
        StartCoroutine(FadeSwapBgm(clip));
    }

    public void StopBGM()
    {
        if (bgmSource == null) return;
        StopAllCoroutines();
        StartCoroutine(FadeOut(bgmSource, fadeTime));
    }

    public void PlaySFX(AudioClip clip, float pitch = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetBgmVolume(float v)
    {
        bgmVolume = Mathf.Clamp01(v);
        if (bgmSource) bgmSource.volume = bgmVolume;
    }

    // === Fades ===
    System.Collections.IEnumerator FadeSwapBgm(AudioClip next)
    {
        // fade out current
        yield return FadeOut(bgmSource, fadeTime);
        bgmSource.clip = next;
        bgmSource.Play();
        yield return FadeIn(bgmSource, fadeTime, bgmVolume);
    }

    System.Collections.IEnumerator FadeOut(AudioSource src, float t)
    {
        if (src == null) yield break;
        float start = src.volume;
        float timer = 0f;
        while (timer < t)
        {
            timer += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, timer / t);
            yield return null;
        }
        src.volume = 0f;
        src.Stop();
    }

    System.Collections.IEnumerator FadeIn(AudioSource src, float t, float target)
    {
        if (src == null) yield break;
        float timer = 0f;
        src.volume = 0f;
        while (timer < t)
        {
            timer += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(0f, target, timer / t);
            yield return null;
        }
        src.volume = target;
    }
}

[System.Serializable]
public class SceneBgmEntry
{
    public string sceneName;   // misal: "day1", "Combat1", "mainmenu"
    public AudioClip clip;     // BGM untuk scene itu
}

