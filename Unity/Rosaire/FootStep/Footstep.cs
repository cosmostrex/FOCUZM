// 2025.10.07 업데이트

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

    [Header("그룹의 마지막 스텝 설정")]
    public bool isFinalStepInGroup;
    public bool isFinalEventTrigger; // isFinalStepInGroup이 true일 때만 의미 있음

    private FootstepManager manager;
    private AudioSource audioSource;
    private bool isTriggered = false;

    private Renderer[] childRenderers;
    private ParticleSystem[] childParticles;
    private GameObject parentGroup;


    void Awake()
    {
        manager = FindObjectOfType<FootstepManager>();
        if (manager == null) Debug.LogError("FootstepManager를 씬에서 찾을 수 없습니다!");

        audioSource = GetComponent<AudioSource>();
        childRenderers = GetComponentsInChildren<Renderer>(true);
        childParticles = GetComponentsInChildren<ParticleSystem>(true);

        // 나의 바로 위 부모를 그룹으로 인식하도록 수정
        
        if (transform.parent != null)
        {
            parentGroup = transform.parent.gameObject;
        }
        else
        {
            Debug.LogError("Footstep 오브젝트는 반드시 그룹 안에 있어야 함", gameObject);
        }

    }

    void OnEnable()
    {
        isTriggered = false;
        if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = true;
        StartCoroutine(FadeIn());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") && !isTriggered)
        {
            isTriggered = true;
            if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

            if (type == FootstepType.Guiding)
            {
                StartCoroutine(FadeOut(gameObject, false));
            }
            else if (type == FootstepType.Narration)
            {
                StartCoroutine(HandleNarration());
            }
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
                manager.StartFinalEvent();
            }
            else
            {
                manager.ShowNextFootstepGroup();
            }

            // 그룹 전체를 비활성화 (Destroy가 아닌 안전한 방식)
            StartCoroutine(FadeOut(parentGroup, false));
        }
        else
        {
            // 그룹의 마지막이 아닌 나레이션 스텝은 자기 자신만 비활성화
            StartCoroutine(FadeOut(gameObject, false));
        }
    }

    #region Fade Effects
    private IEnumerator FadeIn()
    {
        SetAlpha(0f);
        float elapsedTime = 0f;
        while (elapsedTime < effectDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / effectDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(1f);
    }

    private IEnumerator FadeOut(GameObject objectToProcess, bool destroy)
    {
        float elapsedTime = 0f;
        while (elapsedTime < effectDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / effectDuration);
            SetAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f);

        if (destroy)
        {
            Destroy(objectToProcess);
        }
        else
        {
            objectToProcess.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        foreach (var rend in childRenderers)
        {
            if (rend != null && rend.material.HasProperty("_Color"))
            {
                Color newColor = rend.material.color;
                newColor.a = alpha;
                rend.material.color = newColor;
            }
        }
        foreach (var ps in childParticles)
        {
            if (ps != null)
            {
                var main = ps.main;
                var startColor = main.startColor;
                Color newColor = startColor.color;
                newColor.a = alpha;
                startColor.color = newColor;
                main.startColor = startColor;
            }
        }
    }
    #endregion
}
