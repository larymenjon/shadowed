using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Playback")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loopPlaylist = true;

    [Header("Volume")]
    [SerializeField] private float musicVolumeMultiplier = 1f;

    [Header("Scope")]
    [SerializeField] private string levelScenePrefix = "Level_";

    private int currentTrackIndex = -1;
    private bool isLevelScene;

    private void Awake()
    {
        EnsureAudioSource();
        string activeSceneName = SceneManager.GetActiveScene().name;
        isLevelScene = !string.IsNullOrWhiteSpace(activeSceneName) && activeSceneName.StartsWith(levelScenePrefix);
    }

    private void Start()
    {
        ApplyMusicVolume();

        if (!isLevelScene)
            return;

        if (playOnStart)
            PlayPlaylistFromStart();
    }

    private void Update()
    {
        if (!isLevelScene)
            return;

        ApplyMusicVolume();

        if (musicSource == null || playlist == null || playlist.Length == 0)
            return;

        if (musicSource.isPlaying)
            return;

        PlayNextTrack();
    }

    public void PlayPlaylistFromStart()
    {
        EnsurePlaylistFallback();

        if (playlist == null || playlist.Length == 0)
            return;

        currentTrackIndex = 0;
        PlayCurrentTrack();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    private void PlayNextTrack()
    {
        EnsurePlaylistFallback();

        if (playlist == null || playlist.Length == 0)
            return;

        if (currentTrackIndex < 0)
        {
            currentTrackIndex = 0;
            PlayCurrentTrack();
            return;
        }

        int nextIndex = currentTrackIndex + 1;
        if (nextIndex >= playlist.Length)
        {
            if (!loopPlaylist)
                return;

            nextIndex = 0;
        }

        currentTrackIndex = nextIndex;
        PlayCurrentTrack();
    }

    private void PlayCurrentTrack()
    {
        EnsurePlaylistFallback();

        if (musicSource == null || playlist == null || playlist.Length == 0)
            return;

        AudioClip clip = playlist[currentTrackIndex];
        if (clip == null)
        {
            PlayNextTrack();
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
    }

    private void ApplyMusicVolume()
    {
        if (musicSource == null)
            return;

        float optionsVolume = OptionsManager.MusicVolume;
        musicSource.volume = Mathf.Clamp01(optionsVolume * musicVolumeMultiplier);
    }

    private void EnsureAudioSource()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = false;
    }

    private void EnsurePlaylistFallback()
    {
        if (playlist != null && playlist.Length > 0)
            return;

        if (musicSource == null || musicSource.clip == null)
            return;

        playlist = new[] { musicSource.clip };
        loopPlaylist = true;
    }
}
