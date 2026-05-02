using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Video")]
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown graphicsQualityDropdown;
    [SerializeField] private Toggle vSyncToggle;

    [Header("Gameplay")]
    [SerializeField] private Dropdown difficultyDropdown;

    [Header("Accessibility")]
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Toggle subtitlesToggle;

    [Header("Legacy (optional)")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightnessOverlay;
    [SerializeField] private Slider sensitivitySlider;

    private Resolution[] availableResolutions;

    private const string MasterVolumeKey = "Options.MasterVolume";
    private const string MusicVolumeKey = "Options.MusicVolume";
    private const string SfxVolumeKey = "Options.SfxVolume";
    private const string ResolutionIndexKey = "Options.ResolutionIndex";
    private const string FullscreenKey = "Options.Fullscreen";
    private const string GraphicsQualityKey = "Options.GraphicsQuality";
    private const string VSyncKey = "Options.VSync";
    private const string DifficultyKey = "Options.Difficulty";
    private const string LanguageKey = "Options.Language";
    private const string SubtitlesKey = "Options.Subtitles";
    private const string BrightnessKey = "Brightness";
    private const string SensitivityKey = "Sensitivity";

    private void Start()
    {
        BuildResolutionOptions();
        LoadSettings();
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, AudioListener.volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
        PlayerPrefs.Save();
    }

    public void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, Mathf.Clamp01(value));
        PlayerPrefs.Save();
    }

    public void SetResolution(int dropdownIndex)
    {
        if (availableResolutions == null || availableResolutions.Length == 0)
            return;

        int index = Mathf.Clamp(dropdownIndex, 0, availableResolutions.Length - 1);
        Resolution selected = availableResolutions[index];
        Screen.SetResolution(selected.width, selected.height, Screen.fullScreen);

        PlayerPrefs.SetInt(ResolutionIndexKey, index);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        int index = Mathf.Clamp(qualityIndex, 0, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt(GraphicsQualityKey, index);
        PlayerPrefs.Save();
    }

    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        PlayerPrefs.SetInt(VSyncKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetDifficulty(int difficultyIndex)
    {
        PlayerPrefs.SetInt(DifficultyKey, Mathf.Max(0, difficultyIndex));
        PlayerPrefs.Save();
    }

    public void SetLanguage(int languageIndex)
    {
        PlayerPrefs.SetInt(LanguageKey, Mathf.Max(0, languageIndex));
        PlayerPrefs.Save();
    }

    public void SetSubtitles(bool enabled)
    {
        PlayerPrefs.SetInt(SubtitlesKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Placeholder: key bindings system can read this state later.
    public void OpenControls()
    {
        Debug.Log("[OptionsManager] OpenControls acionado. Conecte ao painel de remapeamento.");
    }

    public void SetBrightness(float value)
    {
        if (brightnessOverlay != null)
        {
            Color color = brightnessOverlay.color;
            color.a = 1f - Mathf.Clamp01(value);
            brightnessOverlay.color = color;
        }

        PlayerPrefs.SetFloat(BrightnessKey, Mathf.Clamp01(value));
        PlayerPrefs.Save();
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
    }

    private void BuildResolutionOptions()
    {
        if (resolutionDropdown == null)
            return;

        availableResolutions = Screen.resolutions
            .Distinct(new ResolutionComparer())
            .ToArray();

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(availableResolutions
            .Select(r => $"{r.width} x {r.height} @{r.refreshRateRatio.value:0}Hz")
            .ToList());
    }

    private void LoadSettings()
    {
        float master = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float music = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;
        int quality = PlayerPrefs.GetInt(GraphicsQualityKey, QualitySettings.GetQualityLevel());
        bool vSync = PlayerPrefs.GetInt(VSyncKey, QualitySettings.vSyncCount > 0 ? 1 : 0) == 1;
        int difficulty = PlayerPrefs.GetInt(DifficultyKey, 0);
        int language = PlayerPrefs.GetInt(LanguageKey, 0);
        bool subtitles = PlayerPrefs.GetInt(SubtitlesKey, 1) == 1;
        float brightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);
        float sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);

        if (masterVolumeSlider != null) masterVolumeSlider.value = master;
        if (musicVolumeSlider != null) musicVolumeSlider.value = music;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfx;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        if (graphicsQualityDropdown != null) graphicsQualityDropdown.value = Mathf.Clamp(quality, 0, QualitySettings.names.Length - 1);
        if (vSyncToggle != null) vSyncToggle.isOn = vSync;
        if (difficultyDropdown != null) difficultyDropdown.value = Mathf.Max(0, difficulty);
        if (languageDropdown != null) languageDropdown.value = Mathf.Max(0, language);
        if (subtitlesToggle != null) subtitlesToggle.isOn = subtitles;
        if (brightnessSlider != null) brightnessSlider.value = brightness;
        if (sensitivitySlider != null) sensitivitySlider.value = sensitivity;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSfxVolume(sfx);
        SetFullscreen(fullscreen);
        SetGraphicsQuality(quality);
        SetVSync(vSync);
        SetDifficulty(difficulty);
        SetLanguage(language);
        SetSubtitles(subtitles);
        SetBrightness(brightness);
        SetSensitivity(sensitivity);

        ApplySavedResolution();
    }

    private void ApplySavedResolution()
    {
        if (availableResolutions == null || availableResolutions.Length == 0)
            return;

        int savedIndex = Mathf.Clamp(PlayerPrefs.GetInt(ResolutionIndexKey, availableResolutions.Length - 1), 0, availableResolutions.Length - 1);
        if (resolutionDropdown != null)
            resolutionDropdown.value = savedIndex;

        SetResolution(savedIndex);
    }

    private sealed class ResolutionComparer : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y)
        {
            return x.width == y.width && x.height == y.height;
        }

        public int GetHashCode(Resolution obj)
        {
            unchecked
            {
                return (obj.width * 397) ^ obj.height;
            }
        }
    }
}
