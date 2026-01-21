using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Volume")]
    public Slider volumeSlider;

    [Header("Brightness")]
    public Slider brightnessSlider;
    public Image brightnessOverlay;

    [Header("Sensitivity")]
    public Slider sensitivitySlider;

    private void Start()
    {
        LoadSettings();
    }

    // =====================
    // VOLUME
    // =====================
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    // =====================
    // BRIGHTNESS
    // =====================
    public void SetBrightness(float value)
    {
        if (brightnessOverlay == null) return;

        Color color = brightnessOverlay.color;
        color.a = 1f - value; // slider 1 = claro | 0 = escuro
        brightnessOverlay.color = color;

        PlayerPrefs.SetFloat("Brightness", value);
    }

    // =====================
    // SENSITIVITY
    // =====================
    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
    }

    // =====================
    // LOAD
    // =====================
    private void LoadSettings()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        float brightness = PlayerPrefs.GetFloat("Brightness", 1f);
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1f);

        volumeSlider.value = volume;
        brightnessSlider.value = brightness;
        sensitivitySlider.value = sensitivity;

        SetVolume(volume);
        SetBrightness(brightness);
    }
}

