// 풋스텝 매니저의 경우 로사리오 전체 씬을 관장함.
// 풋스텝 매니저는 씬 매니저의 역할을 하기도 함(추후 추가 예정…)

// 2025.10.07 업데이트

using UnityEngine;
using System.Collections;

public class FootstepManager : MonoBehaviour
{
    [Header("발자국 그룹 배열 (순서대로 등록)")]
    public GameObject[] footstepGroups;

    [Header("그룹 전환 효과 설정")]
    public float showNextGroupDelay = 1.0f;
    public float fadeDuration = 1.5f;

    [Header("마지막 이벤트 설정")]
    public Transform cameraTransform;
    public Vector3 finalCameraRotation;
    public float rotationDuration = 2.0f;

    [Header("최종 이벤트 나레이션")]
    public AudioClip finalNarrationClip;
    public GameObject skipButton;

    private AudioSource audioSource;
    private int currentGroupIndex = 0;
    private bool isProcessing = false;
    private bool isNarrationSkipped = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }
    }

    void Start()
    {
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null) cameraTransform = mainCam.transform;
        }

        if (skipButton != null) skipButton.SetActive(false);

        foreach (GameObject group in footstepGroups)
        {
            if (group != null) group.SetActive(false);
        }

        if (footstepGroups.Length > 0 && footstepGroups[0] != null)
        {
            footstepGroups[0].SetActive(true);
        } // 코루틴 변경
            }

    public void ShowNextFootstepGroup()
    {
        if (isProcessing) return;
        StartCoroutine(ShowNextGroupRoutine());
    }

    public void SkipNarration()
    {
        isNarrationSkipped = true;
    }

    private IEnumerator ShowNextGroupRoutine()
    {
        isProcessing = true;
        currentGroupIndex++;
        if (currentGroupIndex < footstepGroups.Length)
        {
            yield return new WaitForSeconds(showNextGroupDelay);
            if (footstepGroups[currentGroupIndex] != null)
            {
                footstepGroups[currentGroupIndex].SetActive(true);
            }
        }
        isProcessing = false;
    }

    public void StartFinalEvent()
    {
        if (isProcessing) return;
        StartCoroutine(RotateCameraRoutine());
    }

    private IEnumerator RotateCameraRoutine()
    {
        isProcessing = true;
        isNarrationSkipped = false;

        // 1. 벽 사라지기 (회전과 동시에 진행)
        GameObject[] walls = GameObject.FindGameObjectsWithTag("dissolvewall");
        foreach (GameObject wall in walls)
        {
            StartCoroutine(FadeOut(wall));
        }

        // 2. 카메라 회전을 시작하고 끝날 때까지 기다림
        if (cameraTransform != null)
        {
            Quaternion startRotation = cameraTransform.rotation;
            Quaternion targetRotation = Quaternion.Euler(finalCameraRotation);
            float elapsedTime = 0f;
            while (elapsedTime < rotationDuration)
            {
                cameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cameraTransform.rotation = targetRotation;
        }

        yield return new WaitForSeconds(2.0f);

        if (audioSource != null && finalNarrationClip != null)
        {
            audioSource.clip = finalNarrationClip;
            audioSource.Play();
        }

        if (skipButton != null) skipButton.SetActive(true);
        
        while (audioSource.isPlaying && !isNarrationSkipped)
        {
            yield return null;
        }

        if (isNarrationSkipped)
        {
            audioSource.Stop();
            Debug.Log("나레이션 스킵됨!");
        }

        if (skipButton != null) skipButton.SetActive(false);

        Debug.Log("최종 이벤트 완료! (나중에 여기에 씬 전환 로직 추가)");

        isProcessing = false;
    }

    private IEnumerator FadeOut(GameObject objectToFade)
    {
        var renderers = objectToFade.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            objectToFade.SetActive(false);
            yield break;
        }
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            foreach (var rend in renderers)
            {
                if (rend.material.HasProperty("_Color"))
                {
                    Color newColor = rend.material.color;
                    newColor.a = alpha;
                    rend.material.color = newColor;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        objectToFade.SetActive(false);
    }
}
