using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeadsetNarrationController : MonoBehaviour
{
    [Header("UI 페이드 (이전/다음/그만/헤드셋 전체 묶음)")]
    public CanvasGroup canvasGroup;   
    public float appearDelay = 3f;  
    public float fadeDuration = 3f;  

    [Header("헤드셋 버튼")]
    public Button headsetButton;    

    [Header("오디오 (각 GameObject에 AudioSource 하나씩)")]
    [Tooltip("하나의 GameObject 당 AudioSource 하나, PlayOnAwake/Loop 끄기")]
    public AudioSource[] narrationSources; 
    [Tooltip("각 나레이션 사이 텀(초)")]
    public float gapBetweenSources = 2f;

    [Header("페이지 넘김 힌트 UI")]
    [Tooltip("책 위에 띄울 '책을 한 장 넘기세요' 같은 UI에 CanvasGroup 붙여서 넣기")]
    public CanvasGroup turnPageHintGroup;
    [Tooltip("힌트가 켜졌다 꺼지는 페이드 시간")]
    public float hintFadeDuration = 0.4f;
    [Tooltip("완전히 켜진 상태로 잠깐 유지되는 시간")]
    public float hintStayDuration = 0.8f;
    [Tooltip("어느 인덱스의 나레이션 시작 시 힌트를 띄울지 (예: 2,5 등)")]
    public int[] showHintOnClipStart;

    bool canClick = false;        
    bool isPlaying = false;       
    Coroutine playRoutine = null; 
    Coroutine hintRoutine = null; 
    int currentIndex = -1;        

   
    [HideInInspector]
    public bool hasAppeared = false;

    void Awake()
    {
       
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;     
            canvasGroup.blocksRaycasts = false;  
        }

        if (headsetButton != null)
            headsetButton.interactable = true;

       
        if (turnPageHintGroup != null)
        {
            turnPageHintGroup.alpha = 0f;
            turnPageHintGroup.gameObject.SetActive(false);
        }

        hasAppeared = false;
    }

    void OnEnable()
    {
        StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        yield return new WaitForSeconds(appearDelay);

        if (canvasGroup == null) yield break;

        float t = 0f;
        canvasGroup.blocksRaycasts = false;
        hasAppeared = false;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = k;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canClick = true;

      
        hasAppeared = true;
    }

  
    public void OnClickPlayNarration()
    {
        if (!canClick) return;

       
        if (isPlaying)
        {
            Debug.Log("[Narration] 토글 OFF: 재생 중단");
            StopCurrentNarration();
            return;
        }

        if (narrationSources == null || narrationSources.Length == 0)
        {
            Debug.LogWarning("[Narration] narrationSources 배열이 비어있음");
            return;
        }

        playRoutine = StartCoroutine(PlayAllNarrationsBySources());
    }

    void StopCurrentNarration()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        StopAllSources();
        isPlaying = false;
        currentIndex = -1;

        
        if (hintRoutine != null)
        {
            StopCoroutine(hintRoutine);
            hintRoutine = null;
        }
        HideHintImmediate();
    }

    void StopAllSources()
    {
        if (narrationSources == null) return;

        foreach (var src in narrationSources)
        {
            if (src != null)
                src.Stop();
        }
    }

    IEnumerator PlayAllNarrationsBySources()
    {
        isPlaying = true;

        for (int i = 0; i < narrationSources.Length; i++)
        {
            var src = narrationSources[i];
            if (src == null) continue;

            currentIndex = i;

        
            StopAllSources();

            Debug.Log($"[Narration] {i}번 소스 재생: {src.clip?.name}");
            src.Play();

          
            if (ShouldShowHintOnStart(i))
            {
                if (hintRoutine != null)
                    StopCoroutine(hintRoutine);

                hintRoutine = StartCoroutine(FlashTurnPageHintOnce());
            }

           
            while (src.isPlaying)
            {
                yield return null;
            }

           
            if (i < narrationSources.Length - 1 && gapBetweenSources > 0f)
            {
                yield return new WaitForSeconds(gapBetweenSources);
            }
        }

        Debug.Log("[Narration] 전체 재생 완료");
        isPlaying = false;
        currentIndex = -1;
        playRoutine = null;
        hintRoutine = null;
    }

    bool ShouldShowHintOnStart(int clipIndex)
    {
        if (showHintOnClipStart == null || showHintOnClipStart.Length == 0)
            return false;

        return System.Array.IndexOf(showHintOnClipStart, clipIndex) >= 0;
    }

    IEnumerator FlashTurnPageHintOnce()
    {
        if (turnPageHintGroup == null)
            yield break;

        turnPageHintGroup.gameObject.SetActive(true);

        
        float t = 0f;
        while (t < hintFadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / hintFadeDuration);
            turnPageHintGroup.alpha = k;
            yield return null;
        }
        turnPageHintGroup.alpha = 1f;

       
        if (hintStayDuration > 0f)
            yield return new WaitForSeconds(hintStayDuration);

       
        t = 0f;
        while (t < hintFadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / hintFadeDuration);
            turnPageHintGroup.alpha = 1f - k;
            yield return null;
        }
        turnPageHintGroup.alpha = 0f;
        turnPageHintGroup.gameObject.SetActive(false);
    }

    void HideHintImmediate()
    {
        if (turnPageHintGroup == null) return;
        turnPageHintGroup.alpha = 0f;
        turnPageHintGroup.gameObject.SetActive(false);
    }

   
    public IEnumerator FadeOut(float duration)
    {
        if (canvasGroup == null) yield break;

        StopCurrentNarration();
        canClick = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.inte...

<...etc...>
