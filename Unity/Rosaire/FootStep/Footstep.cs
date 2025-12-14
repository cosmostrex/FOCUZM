using UnityEngine;
using System.Collections;

public class Footstep : MonoBehaviour
{
    public enum FootstepType { Guiding, Narration }

    [Header("발자국 타입 설정")]
    public FootstepType type = FootstepType.Guiding;

    [Header("효과 시간")]
    public float effectDuration = 1.5f;

    [Header("나레이션 설정")]
    public AudioClip narrationClip;
    public float narrationPlayTime = 3.0f;

    [Header("그룹/최종 이벤트 트리거")]
    public bool isFinalStepInGroup;
    public bool isFinalEventTrigger;

    private FootstepManager manager;
    private AudioSource audioSource;
    private bool isTriggered = false;

    private Renderer[] childRenderers;
    private ParticleSystem[] childParticles;
    private GameObject parentGroup;

    void Awake()
    {
        manager = FindObjectOfType<FootstepManager>();
        if (manager == null) Debug.LogError("FootstepManager missing");

        audioSource = GetComponent<AudioSource>();
        childRenderers = GetComponentsInChildren<Renderer>(true);
        childParticles = GetComponentsInChildren<ParticleSystem>(true);

        if (transform.parent != null) parentGroup = transform.parent.gameObject;
        else Debug.LogError("Footstep 오브젝트는 반드시 그룹 아래에 있어야 함", gameObject);
    }

    void OnEnable()
    {
        isTriggered = false;
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
        StartCoroutine(FadeIn());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") && !isTriggered)
        {
            isTriggered = true;
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            if (type == FootstepType.Guiding)
                StartCoroutine(FadeOut(gameObject, false));
            else if (type == FootstepType.Narration)
                StartCoroutine(HandleNarration());
        }
    }

    private IEnumerator HandleNarration()
    {
        if (audioSource != null && narrationClip != null)
        {
            audioSource.PlayOneShot(narrationClip);
            yield return new WaitForSeconds(narrationPlayTime);
        }

        if (isFinalStepInGroup)
        {
            if (isFinalEventTrigger)
            {
                // foot3 도달 --> 3초 대기 후 최종 이벤트 시작
                manager.StartFinalEvent(3.0f);
            }
            else
            {
                manager.ShowNextFootstepGroup();
            }
            StartCoroutine(FadeOut(parentGroup, false)); // 그룹 전체 비활성
        }
        else
        {
            StartCoroutine(FadeOut(gameObject, false));  // 현재 발자국만 비활성
        }
    }

    #region Fade Effects
    private IEnumerator FadeIn()
    {
        SetAlpha(0f);
        float t = 0f;
        while (t < effectDuration)
        {
            SetAlpha(Mathf.Lerp(0f, 1f, t / effectDuration));
            t += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1f);
    }

    private IEnumerator FadeOut(GameObject objectToProcess, bool destroy)
    {
        float t = 0f;
        while (t < effectDuration)
        {
            SetAlpha(Mathf.Lerp(1f, 0f, t / effectDuration));
            t += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f);

        if (destroy) Destroy(objectToProcess);
        else objectToProcess.SetActive(false);
    }

    private void SetAlpha(float alpha)
    {
        foreach (var rend in childRenderers)
        {
            if (rend != null && rend.material.HasProperty("_Color"))
            {
                Color c = rend.material.color; c.a = alpha; rend.material.color = c;
            }
        }
        foreach (var ps in childParticles)
        {
            if (ps != null)
            {
                var main = ps.main;
                var sc = main.startColor;
                Color c = sc.color; c.a = alpha; sc.color = c; main.startColor = sc;
            }
        }
    }
    #endregion
}
