using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM 오디오 소스")]
    public AudioSource bgmSource;

    [Header("효과음 오디오 소스 (여러개 재생 가능)")]
    public AudioSource sfxSourcePrefab;  // 프리팹으로 미리 등록
    private List<AudioSource> sfxSources = new List<AudioSource>();

    [Header("오디오 클립 목록")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();

    void Awake()
    {
        // 싱글톤
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        // 오디오 클립 딕셔너리 초기화
        foreach (AudioClip clip in bgmClips)
            bgmDict[clip.name] = clip;

        foreach (AudioClip clip in sfxClips)
            sfxDict[clip.name] = clip;
    }

    // ===== BGM =====
    public void PlayBGM(string clipName, bool loop = true)
    {
        if (bgmDict.ContainsKey(clipName))
        {
            bgmSource.clip = bgmDict[clipName];
            bgmSource.loop = loop;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ===== SFX =====
    public void PlaySFX(string clipName)
    {
        if (sfxDict.ContainsKey(clipName))
        {
            AudioSource sfx = Instantiate(sfxSourcePrefab, transform);
            sfx.clip = sfxDict[clipName];
            sfx.Play();
            Destroy(sfx.gameObject, sfx.clip.length);
        }
    }
}
