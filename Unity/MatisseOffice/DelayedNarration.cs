using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class DelayedNarration : MonoBehaviour
{
    [Header("재생할 오디오 소스")]
    public AudioSource audioSource;

    [Header("씬 시작 후, 나레이션 재생까지 딜레이(초)")]
    public float delayBeforePlay = 3f;

    [Header("나레이션이 끝난 뒤 다음 씬으로 넘어가기까지 딜레이(초)")]
    public float delayAfterEnd = 3f;

    [Header("다음으로 이동할 씬 이름")]
    public string nextSceneName = "SampleScene";

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
            audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        StartCoroutine(PlayAndChangeScene());
    }

    IEnumerator PlayAndChangeScene()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[DelayedNarration] AudioSource가 없음");
            yield break;
        }

        
        if (delayBeforePlay > 0f)
            yield return new WaitForSeconds(delayBeforePlay);

        
        audioSource.Play();

        
        while (audioSource.isPlaying)
            yield return null;

        
        if (delayAfterEnd > 0f)
            yield return new WaitForSeconds(delayAfterEnd);

        
        if (FadeQuadController.Instance != null)
        {
            yield return StartCoroutine(FadeQuadController.Instance.FadeOut());
        }

        
        SceneManager.LoadScene(nextSceneName);
    }
}
