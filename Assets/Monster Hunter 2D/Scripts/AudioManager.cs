using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume = 1f;
    public int bgmChannels;
    public AudioSource[] bgmPlayers;
    int bgmChannelIndex;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume = 1f;
    public int sfxChannels;
    public AudioSource[] sfxPlayers;
    int sfxChannelIndex;

    public enum BGM { TitleBGM, VillageBGM, QuestStart, NatureAudio, RathalosBGM, HunterDead, QuestClear };
    public enum SFX { MenuClick1, MenuClick2, MenuClick3, HunterDepart, Potion, Whetstone, Charge, ChargeMax, Guard, RathalosRoar, RathalosBite, RathalosBreath, RathalosClaw, RathalosTail, RathalosStun, HunterHit, RathalosHit };

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Init();
        PreloadBGM();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayers = new AudioSource[bgmChannels];

        for (int index = 0; index < bgmPlayers.Length; index++)
        {
            bgmPlayers[index] = bgmObject.AddComponent<AudioSource>();
            bgmPlayers[index].playOnAwake = false;
            bgmPlayers[index].volume = bgmVolume;
        }        

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[sfxChannels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    void PreloadBGM()
    {
        float startVolume = bgmVolume;

        for (int index = 0; index < bgmClips.Length; index++)
        {
            bgmPlayers[0].clip = bgmClips[index];
            bgmPlayers[0].volume = 0f;
            bgmPlayers[0].Play();
            bgmPlayers[0].Stop();
        }

        bgmPlayers[0].volume = startVolume;
    }

    public void PlayBGM(BGM bgm)
    {
        for (int index = 0; index < bgmPlayers.Length; index++)
        {
            int loopIndex = (index + bgmChannelIndex) % bgmPlayers.Length;

            if (bgmPlayers[loopIndex].isPlaying) continue;

            bgmChannelIndex = loopIndex;
            bgmPlayers[loopIndex].clip = bgmClips[(int)bgm];

            if (bgm == BGM.TitleBGM || bgm == BGM.VillageBGM || bgm == BGM.NatureAudio || bgm == BGM.RathalosBGM)
            {
                bgmPlayers[loopIndex].loop = true;
            }
            else
            {
                bgmPlayers[loopIndex].loop = false;
            }
            bgmPlayers[loopIndex].Play();
            break;
        }
    }

    public void StopBGM()
    {
        foreach (var player in bgmPlayers) player.Stop();
    }

    public IEnumerator BGMFadeOut()
    {
        float duration = 1f;
        float startVolume = bgmVolume;

        foreach(var player in bgmPlayers)
        {
            if (player.isPlaying)
            {
                for (float t = 0; t < duration; t += Time.deltaTime)
                {
                    player.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                    yield return null;
                }
                player.Stop();
                player.volume = startVolume;
            }
        }
    }

    public void PlaySFX(SFX sfx)
    {
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + sfxChannelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying) continue;

            sfxChannelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(HandleSceneBGM(scene));
    }

    IEnumerator HandleSceneBGM(Scene scene)
    {
        yield return StartCoroutine(BGMFadeOut());

        if (scene.name == "TitleScene") PlayBGM(BGM.TitleBGM);
        else if (scene.name == "VillageScene") PlayBGM(BGM.VillageBGM);
        else if (scene.name == "QuestScene")
        {
            PlayBGM(BGM.QuestStart);
            PlayBGM(BGM.NatureAudio);
        }
    }
}
