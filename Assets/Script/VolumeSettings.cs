using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("Referensi Slider")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    void Start()
    {

        LoadVolumeSettings();

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void LoadVolumeSettings()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.75f);
        bgmSlider.value = bgmVolume;
        AudioManager.Instance.ChangeBGMVolume(bgmVolume);

        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.75f);
        sfxSlider.value = sfxVolume;
        AudioManager.Instance.ChangeSFXVolume(sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        AudioManager.Instance.ChangeBGMVolume(volume);
        PlayerPrefs.SetFloat(BGM_VOLUME_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.ChangeSFXVolume(volume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
    }
}