
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class AudioManager : SerializedMonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private Dictionary<AudioEnums, AudioClip> _clips;

    public AudioSource SFXSource, BGSource;

    public static bool LevelEndSoundCheck;

    public int SoundOnOffCheck
    {
        get
        {
            return PlayerPrefs.GetInt("SoundOnOffCheck", 0);
        }
        set
        {
            PlayerPrefs.SetInt("SoundOnOffCheck", value);
        }
    }
    public int VibroOnOffCheck
    {
        get
        {
            return PlayerPrefs.GetInt("VibroOnOffCheck", 0);
        }
        set
        {
            PlayerPrefs.SetInt("VibroOnOffCheck", value);
        }
    }
    public int MusicOnOffCheck
    {
        get
        {
            return PlayerPrefs.GetInt("MusicOnOffCheck", 0);
        }
        set
        {
            PlayerPrefs.SetInt("MusicOnOffCheck", value);
        }
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PlayBGMusic();
    }
    public void PlayBGMusic()
    {
        if (MusicOnOffCheck == 0) BGSource.Play();
    }
    public void StopBGMusic()
    {
        BGSource.Stop();
    }

    public void Play(AudioEnums audioName, float volume = .5f, bool isPitchRandom = false)
    {
        if (SoundOnOffCheck != 0) return;
        if (LevelEndSoundCheck) return;

        if ( _clips.ContainsKey(audioName)) SFXSource.PlayOneShot(_clips[audioName], volume);
        if (isPitchRandom) SFXSource.pitch = Random.Range(1f, 2f);
        else SFXSource.pitch = 1;
    }
    public void PlayHaptic(HapticPatterns.PresetType presetType)
    {
        if (VibroOnOffCheck == 0) HapticPatterns.PlayPreset(presetType);
    }

    public void Stop()
    {
        SFXSource.Stop();
    }
    public enum AudioEnums
    {
        Tap,
        Swish,
        WrongMove,
        HexagonClear,
        HexagonBounce,
        HexagonHolderPlaced,
        LevelEnd,
        LevelFail,
        Button,
        LockUnlocked,
        ButtonTap,
        HintTap,
        TargetCompleted,
        Rattle,
        Aquatic,
        Cut,
        SkillHammer,
        Boxy,
        Succes1,
        BlenderPush,
        CoinArrived,
        CoinPop,
        BlenderFall,
        Pop,
        LevelEnd2,
        TickTock,
        ExtraTime,
        BlenderReset,
        Combo,
        MetaProgress,
        FingerTap,
        Cut3,
        ButtonTap2,
        OppFound,
        Three,
        Two,
        One,
        End,
        BarSound
    }
}
