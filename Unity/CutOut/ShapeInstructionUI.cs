using System.Collections;
using UnityEngine;

public class ShapeInstructionUI : MonoBehaviour
{
    [Header("ù �ȳ� ���� (���� ���� �ȳ�)")]
    public CanvasGroup firstText;  
    [Header("�� ��° �ȳ� ���� (������ �׸��� ����)")]
    public CanvasGroup secondText; 
    [Header("Ÿ�̹� ����")]
    public float fadeTime = 0.4f;   
    public float showTime = 1.2f;   
    public float gapBetween = 0.3f; 
    void Awake()
    {
        ResetCanvasGroup(firstText);
        ResetCanvasGroup(secondText);
    }

    void ResetCanvasGroup(CanvasGroup cg)
    {
        if (cg == null) return;

        cg.alpha = 0f;           
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void PlaySequence()
    {
        StopAllCoroutines();
        StartCoroutine(InstructionSequence());
    }

    IEnumerator InstructionSequence()
    {
        
        if (firstText != null)
        {
            yield return StartCoroutine(FadeCanvas(firstText, 0f, 1f, fadeTime));
            yield return new WaitForSeconds(showTime);
            yield return StartCoroutine(FadeCanvas(firstText, 1f, 0f, fadeTime));
        }

       
        if (gapBetween > 0f)
            yield return new WaitForSeconds(gapBetween);

        if (secondText != null)
        {
            yield return StartCoroutine(FadeCanvas(secondText, 0f, 1f, fadeTime));
            yield return new WaitForSeconds(showTime);
            yield return StartCoroutine(FadeCanvas(secondText, 1f, 0f, fadeTime));
        }
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null || duration <= 0f)
        {
            if (cg != null) cg.alpha = to;
            yield break;
        }

        float t = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(from, to, k);
            yield return null;
        }

        cg.alpha = to;
    }
}
