using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class HeadsetHoverHintUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ȣ�� �� ��� ���� �г� UI (CanvasGroup �ʼ�)")]
    public CanvasGroup hintGroup;               

    [Header("���̵� ����")]
    public float fadeDuration = 0.3f;

    [Header("���� �����̼� ��Ʈ�ѷ�")]
    public HeadsetNarrationController narration;  

    Coroutine fadeRoutine;
    bool isOver = false;

    void Awake()
    {
        if (hintGroup != null)
        {
            hintGroup.alpha = 0f;
            hintGroup.gameObject.SetActive(false); 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOver = true;

       
        if (narration != null && !narration.hasAppeared)
            return;

        StartFade(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOver = false;

        if (narration != null && !narration.hasAppeared)
            return;

        StartFade(false);
    }

    void StartFade(bool show)
    {
        if (hintGroup == null) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeHint(show));
    }

    IEnumerator FadeHint(bool show)
    {
        if (hintGroup == null) yield break;

        if (show && !hintGroup.gameObject.activeSelf)
            hintGroup.gameObject.SetActive(true);

        float startA = hintGroup.alpha;
        float targetA = show ? 1f : 0f;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);
            hintGroup.alpha = Mathf.Lerp(startA, targetA, k);
            yield return null;
        }

        hintGroup.alpha = targetA;

        if (!show)
            hintGroup.gameObject.SetActive(false);

        fadeRoutine = null;
    }

    
    public void HideImmediate()
    {
        if (hintGroup == null) return;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        hintGroup.alpha = 0f;
        hintGroup.gameObject.SetActive(false);
    }
}
 
