using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ScriptBoy.ProceduralBook;

public class BookMover : MonoBehaviour
{
    [Header("도착 위치 및 스케일")]
    public Vector3 targetPosition = new Vector3(-2.299f, 0.822f, 2.315f);
    public Vector3 targetRotation = new Vector3(-68.76f, 90f, 0f);
    public Vector3 targetScale = new Vector3(0.2f, 0.2f, 0.2f);

    [Header("애니메이션 시간 설정")]
    public float moveDuration = 1.0f;
    public float startDelay = 3.0f;   
    public float arcHeight = 0.2f;  

    [Header("페이지 넘김 데모 (책 레퍼런스용, 없어도 됨)")]
    public AutoTurningDemo autoTurning;

    [Header("UI (CanvasGroup 권장)")]
    public CanvasGroup uiCanvasGroup;  
    [Tooltip("이전 페이지 버튼")]
    public Button prevButton;
    [Tooltip("다음 페이지 버튼")]
    public Button nextButton;
    [Tooltip("그만보기 버튼")]
    public Button stopButton;
    public float uiFadeDuration = 1.0f;

    [Header("사용자 입력 없을 때 책 복귀 (지금은 사용 안 함, 그냥 인스펙터 보존용)")]
    public float idleSeconds = 3.0f;

    [Header("Shape (대표도형 부모 오브젝트)")]
    public GameObject shapeGroup;

    [Header("도형 상승 매니저")]
    public ShapeRiseManager shapeRiseManager;

    [Header("헤드셋 나레이션 (선택사항)")]
    public HeadsetNarrationController headsetNarration;

   
    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalScale;

    bool movedToFront = false; 
    bool finishStarted = false; 

    void Awake()
    {
       
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalScale = transform.localScale;
    }

    void Start()
    {
        
        if (shapeGroup != null)
            shapeGroup.SetActive(false);

       
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }

       
        if (prevButton != null) prevButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
        if (stopButton != null) stopButton.interactable = false;

       
        Invoke(nameof(StartMovement), startDelay);
    }

    void OnEnable()
    {
        if (stopButton != null)
            stopButton.onClick.AddListener(OnClickStop);
    }

    void OnDisable()
    {
        if (stopButton != null)
            stopButton.onClick.RemoveListener(OnClickStop);
    }

   
    public void StartMovement()
    {
        StartCoroutine(MoveToFrontSequence());
    }

    IEnumerator MoveToFrontSequence()
    {
       
        yield return StartCoroutine(
            MoveToTarget(targetPosition, targetRotation, targetScale, moveDuration, useArc: true)
        );

        movedToFront = true;

       
        if (uiCanvasGroup != null)
            yield return StartCoroutine(FadeInUI(uiCanvasGroup, uiFadeDuration));

      
        if (prevButton != null) prevButton.interactable = true;
        if (nextButton != null) nextButton.interactable = true;
        if (stopButton != null) stopButton.interactable = true;
    }

  
    public void OnClickStop()
    {
        if (!movedToFront || finishStarted) return;
        StartCoroutine(FinishSequence());
    }

    IEnumerator FinishSequence()
    {
        if (finishStarted) yield break;
        finishStarted = true;

        if (prevButton != null) prevButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
        if (stopButton != null) stopButton.interactable = false;

        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

   
        Coroutine moveBackCo = StartCoroutine(
            MoveToTarget(originalPosition, originalRotation.eulerAngles, originalScale, moveDuration, useArc: false)
        );

      
        Coroutine fadeUICo = null;
        if (uiCanvasGroup != null)
            fadeUICo = StartCoroutine(FadeOutUI(uiCanvasGroup, uiFadeDuration));

        Coroutine fadeHeadsetCo = null;
        if (headsetNarration != null)
            fadeHeadsetCo = StartCoroutine(headsetNarration.FadeOut(uiFadeDuration));

       
        yield return moveBackCo;

        if (fadeUICo != null) yield return fadeUICo;
        if (fadeHeadsetCo != null) yield return fadeHeadsetCo;

      
        if (shapeGroup != null)
        {
            shapeGroup.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }

        if (shapeRiseManager != null)
            shapeRiseManager.StartRise();

        Debug.Log("BookMover: 책 복귀 + UI 헤드셋 페이드아웃 + 도형 상승 시퀀스 완료");
    }

 
    IEnumerator FadeInUI(CanvasGroup cg, float fadeTime)
    {
        if (!cg.gameObject.activeSelf)
            cg.gameObject.SetActive(true);

        float startAlpha = cg.alpha;
        float t = 0f;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            cg.alpha = Mathf.Lerp(startAlpha, 1f, k);
            yield return null;
        }

        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    IEnumerator FadeOutUI(CanvasGroup cg, float fadeTime)
    {
        if (!cg.gameObject.activeSelf) yield break;

        float startAlpha = cg.alpha;
        float t = 0f;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            cg.alpha = Mathf.Lerp(startAlpha, 0f, k);
            yield return null;
        }

        cg.alpha = 0f;
        cg.gameObject.SetActive(false);
    }


    IEnumerator MoveToTarget(Vector3 endPos, Vector3 endRot, Vector3 endScale, float time, bool useArc)
    {
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        Vector3 startScale = transform.localScale;
        Quaternion endQuat = Quaternion.Euler(endRot);

        float elapsed = 0f;

     ...

<...etc...>
