using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class IntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public TextMeshProUGUI touchText;

    [Header("Siguiente Escena")]
    public string mainMenuSceneName = "MainMenu";

    private bool isTransitioning = false;
    private bool canSkip = false;

    void Start()
    {

        Time.timeScale = 1f;

        if (touchText != null) StartCoroutine(BlinkTextRoutine());

        if (videoPlayer != null)
        {
            StartCoroutine(PrepareAndPlayVideo());
        }

        Invoke(nameof(EnableSkip), 1.5f);
    }

    private IEnumerator PrepareAndPlayVideo()
    {
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
    }

    void EnableSkip()
    {
        canSkip = true;
    }

    void Update()
    {
        if (isTransitioning || !canSkip) return;

        bool touched = false;

        if (Input.GetMouseButtonDown(0))
        {
            touched = true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touched = true;
        }

        if (touched)
        {
            GoToMenu();
        }
    }

    private void GoToMenu()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (videoPlayer != null) videoPlayer.Stop();

        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.LoadScene(mainMenuSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    private IEnumerator BlinkTextRoutine()
    {
        while (true)
        {
            touchText.enabled = true;
            yield return new WaitForSecondsRealtime(0.6f);
            touchText.enabled = false;
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }
}