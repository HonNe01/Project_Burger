using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header(" # BGM")]
    public AudioClip mainBGMClip;
    public AudioClip gameBGMClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header(" # SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;

    int channelsIndex;

    public enum SFX
    {
        // System : 0 ~
        Click, Success, Fail, Bell,

        // Coocking : 4 ~
        Pick, Patty, Patty_Burn,

        // Customer : 7 ~
        Order, Good, Angry,
        Order_Kid, Good_Kid, Angry_Kid,
        Order_Girl, Good_Girl, Angry_Girl,
    }

    void Awake()
    {
        // 인스턴스 초기화
        if (instance != null && instance != this)
        {
            Destroy(gameObject);

            return;
        }

        instance = this;
    }

    void Start()
    {
        Init();

        PlayMainBGM();
    }

    void Init()
    {
        // BGM Init
        GameObject bgmObject = new GameObject("BGM Player");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        // SFX Init
        GameObject sfxObject = new GameObject("SFX Player");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    // ===== BGM =====
    public void PlayMainBGM()
    {
        if (bgmPlayer.clip == mainBGMClip && bgmPlayer.isPlaying) return;

        bgmPlayer.clip = mainBGMClip;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.loop = true;
        bgmPlayer.Play();
        StartCoroutine(Co_Fade(bgmPlayer));
    }
    
    public void PlayGameBGM()
    {
        if (bgmPlayer.clip == gameBGMClip && bgmPlayer.isPlaying) return;

        bgmPlayer.clip = gameBGMClip;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.loop = true;
        bgmPlayer.Play();
        StartCoroutine(Co_Fade(bgmPlayer));
    }

    public void StopSFX(SFX sfx)
    {
        AudioClip target = sfxClips[(int)sfx];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            var sfxPlayer = sfxPlayers[i];
            if (sfxPlayer.isPlaying && sfxPlayer.clip == target)
            {
                sfxPlayer.Stop();
            }
        }
    }

    // ===== SFX =====
    public void PlaySFX(SFX sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelsIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            /*
            if (sfx == SFX.etc || sfx == SFX.etc) 
            {
                ranIndex = Random.Range(0, 2);
            }
            */

            channelsIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();

            break;
        }
    }

    public void PlayCustomerSFX(SFX sfx, CustomerType type)
    {
        int soundOffset = 0;

        switch (type)
        {
            case CustomerType.Default:
                soundOffset = 0;
                break;
            case CustomerType.Kid:
                soundOffset = 3;
                break;
            case CustomerType.Girl:
                soundOffset = 6;
                break;
        }

        if ((int)sfx < sfxClips.Length)
        {
            PlaySFX((SFX)((int)sfx + soundOffset));
        }
    }
    
    private IEnumerator Co_Fade(AudioSource source)
    {
        source.volume = 0f;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            source.volume = Mathf.Lerp(0f, bgmVolume, t);
            yield return null;
        }

        source.volume = bgmVolume;
    }
}


