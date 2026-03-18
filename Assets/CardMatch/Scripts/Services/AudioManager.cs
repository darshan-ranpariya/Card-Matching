using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager inst;

    [Header("Audio Clips")]
    public AudioClip cardFlipSound;
    public AudioClip matchSound;
    public AudioClip comboSound;
    public AudioClip mismatchSound;
    public AudioClip winSound;
    public AudioClip gameOverSound;
    public AudioClip buttonClickSound;

    [Header("Audio Sources")]
    private AudioSource sfxSource;
    private AudioSource musicSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSources()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public void PlayCardFlip()
    {
        PlaySound(cardFlipSound);
    }

    public void PlayMatch()
    {
        PlaySound(matchSound);
    }

    public void PlayCombo()
    {
        PlaySound(comboSound);
    }

    public void PlayMismatch()
    {
        PlaySound(mismatchSound);
    }

    public void PlayWin()
    {
        PlaySound(winSound);
    }

    public void PlayGameOver()
    {
        PlaySound(gameOverSound);
    }

    public void PlayButtonClick()
    {
        PlaySound(buttonClickSound);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
}
