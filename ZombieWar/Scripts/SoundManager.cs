using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 재생할 Audio 이름 상수화 클래스
/// </summary>
public class AudioNameConstant
{
    public const string MAIN_SOUND = "TitleBGM";
    public const string BATTLE_SOUND = "BattleBGM";
    public const string RELOAD_SOUND = "Reload";
    public const string EMPTY_SHELL_SOUND = "EmptyShell";
    public const string GRENADE_SOUND = "Grenade";
    public const string Zombie_Death_SOUND = "ZombieDeath";
}

/// <summary>
/// 클립 정보 데이터 클래스
/// </summary>
[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
}

public class SoundManager : MonoBehaviour
{
    public const float OTHER_SFX_SOUND_VOLUME = 0.2f;  // 다른 플레이어 사운드 볼륨

    [SerializeField] AudioSource bgmSound;  // BGM 재생 AudioSource
    [SerializeField] Sound[] bgmClip;       // BGM에 사용될 clip

    [SerializeField] List<AudioSource> sfxSounds = new List<AudioSource>(); // SFX 재생 AudioSource
    [SerializeField] Sound[] sfxClip;                                       // SFX에 사용될 clip

    // 설정한 이름을 통해 클립을 빨리 찾기위해 dictionary 사용
    // 이름과 클립을 분류
    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public bool IsMute { get; set; } = true;    // 사운드 on/off

    string currentBGM;                          // 최근 재생된 BGM
    public string CurrentBGM
    {
        get => currentBGM;
    }

    private void Awake()
    {
        // bgm 클립 적재
        for (int i = 0; i < bgmClip.Length; i++)
        {
            audioClips.Add(bgmClip[i].soundName, bgmClip[i].clip);
        }

        // sfx 클립 적재
        for (int i = 0; i < sfxClip.Length; i++)
        {
            audioClips.Add(sfxClip[i].soundName, sfxClip[i].clip);
        }
    }

    /// <summary>
    /// BGM 재생 함수
    /// </summary>
    /// <param name="clipName">재생할 clip 이름</param>
    public void PlayBGM(string clipName)
    {
        // mute 설정이면 리턴
        if (!IsMute)
            return;

        // 등록되어있는 클립인지 확인
        if (!audioClips.ContainsKey(clipName))
        {
            Debug.Log("클립이 존재하지 않습니다. clipName: " + clipName);
            return;
        }

        // BGM 재생
        bgmSound.clip = audioClips[clipName];
        bgmSound.Play();

        // 최근 재생된 클립 이름 저장
        currentBGM = clipName;
    }

    /// <summary>
    /// BGM 정지 함수
    /// </summary>
    public void StopBGM()
    {
        // 재생중이라면 정지 실행
        if (bgmSound.isPlaying)
            bgmSound.Stop();
    }

    /// <summary>
    /// SFX 재생 함수
    /// </summary>
    /// <param name="clipName">재생할 clip 이름</param>
    public void PlaySFX(string clipName, float volume = 1f)
    {
        // mute 설정이면 리턴
        if (!IsMute)
            return;

        // 등록되어있는 클립인지 확인
        if (!audioClips.ContainsKey(clipName))
        {
            Debug.Log("클립이 존재하지 않습니다. clipName: " + clipName);
            return;
        }

        for (int i = 0; i < sfxSounds.Count; i++)
        {
            // 사용중이 아닌 SFX전용 AudioSource를 찾아서 재생
            if(!sfxSounds[i].isPlaying)
            {
                sfxSounds[i].clip = audioClips[clipName];
                sfxSounds[i].volume = volume;
                sfxSounds[i].Play();
                return;
            }
        }

        Debug.Log("재생 가능한 Audio Source가 없습니다. clipName: " + clipName);

        // 재생 가능한 SFX전용 AudioSource가 없다면 생성 후 재생
        AudioSource newAudio = transform.GetChild(1).gameObject.AddComponent<AudioSource>();
        newAudio.playOnAwake = false;
        newAudio.clip = audioClips[clipName];
        newAudio.Play();

        sfxSounds.Add(newAudio);
    }
}
