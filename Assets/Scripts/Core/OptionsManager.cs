using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TMP_Text volumeNumberText;
    [SerializeField] private TMP_Text volumeNumberSfxText;

    public const string MusicVolumeKey = "Options.MusicVolume";
    public const string SfxVolumeKey = "Options.SfxVolume";

    public static float MusicVolume => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    public static float SfxVolume => PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

    private void Start()
    {
        BindSliderEvents();
        LoadSettings();
    }

    public void SetMusicVolume(float value)
    {
        float clamped = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, clamped);
        PlayerPrefs.Save();
        UpdateMusicVolumeLabel(clamped);
    }

    public void SetSfxVolume(float value)
    {
        float clamped = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, clamped);
        PlayerPrefs.Save();
        UpdateSfxVolumeLabel(clamped);
    }

    private void LoadSettings()
    {
        float music = MusicVolume;
        float sfx = SfxVolume;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = music;

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfx;

        SetMusicVolume(music);
        SetSfxVolume(sfx);
    }

    private void UpdateMusicVolumeLabel(float normalizedValue)
    {
        if (volumeNumberText == null)
            return;

        int percentage = Mathf.RoundToInt(Mathf.Clamp01(normalizedValue) * 100f);
        volumeNumberText.text = percentage.ToString();
    }

    private void UpdateSfxVolumeLabel(float normalizedValue)
    {
        if (volumeNumberSfxText == null)
            return;

        int percentage = Mathf.RoundToInt(Mathf.Clamp01(normalizedValue) * 100f);
        volumeNumberSfxText.text = percentage.ToString();
    }

    private void BindSliderEvents()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
            sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }
}
