using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Daftar Audio")]
    [SerializeField] private Sound[] bgmSounds;
    [SerializeField] private Sound[] sfxSounds;

    [Header("Sumber Audio")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    public AudioSource BGMSource { get { return bgmSource; } }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string soundName)
    {
        Sound s = Array.Find(bgmSounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("BGM dengan nama: " + soundName + " tidak ditemukan!");
            return;
        }
        bgmSource.clip = s.clip;
        bgmSource.Play();
    }

    public void PlaySFX(string soundName)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("SFX dengan nama: " + soundName + " tidak ditemukan!");
            return;
        }
        sfxSource.PlayOneShot(s.clip);
    }

    public void ChangeBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    public void ChangeSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}