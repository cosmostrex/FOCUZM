using UnityEngine;
using System.Collections;

public class BGMFader : MonoBehaviour
{
    public static BGMFader Instance;

    private AudioSource audioSource;
    public float fadeDuration = 2f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    public void PlayBGM(AudioClip clip)
    {
        StartCoroutine(FadeToNewBGM(clip));
    }

    private IEnumerator FadeToNewBGM(AudioClip newClip)
    {
        float startVolume = audioSource.volume;

        
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.clip = newClip;
        audioSource.Play();

        
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 1f;
    }
}
