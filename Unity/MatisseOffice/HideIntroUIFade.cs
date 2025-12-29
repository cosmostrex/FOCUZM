
using UnityEngine;
using System.Collections;

public class HideIntroUIFade : MonoBehaviour
{
    [Header("�� �� �ڿ� ���̵�ƿ� ��������")]
    public float delay = 5f;          

    [Header("���̵�ƿ�� �ɸ��� �ð�")]
    public float fadeDuration = 1.5f; 

    [Header("���̵�ƿ� ��ų UI ��Ʈ ������Ʈ")]
    public GameObject targetUI;      

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        
        if (targetUI == null)
            targetUI = gameObject;


        canvasGroup = targetUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = targetUI.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void Start()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        
        if (delay > 0f)
            yield return new WaitForSeconds(delay);


        float t = 0f;
        float startAlpha = canvasGroup.alpha;


        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, ratio);
            yield return null;
        }


        canvasGroup.alpha = 0f;
        targetUI.SetActive(false);
    }
}
