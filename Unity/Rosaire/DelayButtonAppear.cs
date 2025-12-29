using UnityEngine;

public class DelayButtonAppear : MonoBehaviour
{
    [Header("버튼 UI 오브젝트")]
    public GameObject buttonUI;

    [Header("딜레이 시간 (초)")]
    public float delayTime = 2f;

    [Header("재생할 오디오")]
    public AudioSource narrationAudio;

    void Start()
    {
        if (buttonUI != null)
            buttonUI.SetActive(false);

        StartCoroutine(ShowButtonRoutine());
    }

    System.Collections.IEnumerator ShowButtonRoutine()
    {
        yield return new WaitForSeconds(delayTime);

        if (buttonUI != null)
            buttonUI.SetActive(true);

        if (narrationAudio != null)
            narrationAudio.Play();
    }
}
