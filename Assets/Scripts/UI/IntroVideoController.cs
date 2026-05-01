using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class IntroVideoController : MonoBehaviour
{
    [Header("Flow")]
    [SerializeField] private string nextSceneName = "MainMenu";
    [SerializeField] private bool allowSkip = true;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    private bool isLoading;

    private void Reset()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            LoadNextScene();
            return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.errorReceived += OnVideoError;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (videoPlayer.clip == null)
        {
            Debug.LogWarning("[IntroVideoController] Nenhum VideoClip configurado. Pulando intro.");
            LoadNextScene();
            return;
        }

        videoPlayer.Play();
    }

    private void Update()
    {
        if (!allowSkip || isLoading)
            return;

        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            LoadNextScene();
    }

    private void OnDestroy()
    {
        if (videoPlayer == null)
            return;

        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.errorReceived -= OnVideoError;
    }

    private void OnVideoFinished(VideoPlayer _)
    {
        LoadNextScene();
    }

    private void OnVideoError(VideoPlayer _, string message)
    {
        Debug.LogWarning($"[IntroVideoController] Erro ao tocar vídeo: {message}. Pulando intro.");
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        if (isLoading)
            return;

        isLoading = true;
        SceneManager.LoadScene(nextSceneName);
    }
}

