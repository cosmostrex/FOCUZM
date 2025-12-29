using UnityEngine;

public class DelayedAudioPlayer : MonoBehaviour
{
    [Header("a")]
    public AudioSource audioSource;

    [Header("a (a")]
    public float delay = 2f;

    private void Start()
    {
        if (audioSource != null)
        {
            StartCoroutine(PlayAfterDelay());
        }
    }

    private System.Collections.IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}
