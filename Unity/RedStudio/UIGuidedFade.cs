using UnityEngine;
using System.Collections;

public class UIGuidedFade : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeInTime = 0.5f;
    public float showDuration = 2f;
    public float fadeOutTime = 0.5f;

    [Header("디버그용")]
    public bool playOnStart = false;

    private CanvasGroup cg;
    private Coroutine routine;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg)
        {
            cg = gameObject.AddComponent<CanvasGroup>();
        }

      
        cg.alpha = 0f;
    }

    void Start()
    {
        if (playOnStart)
        {
            Debug.Log("[UIGuidedFade] playOnStart");
            Play();
        }
    }

    public void Play()
    {
       
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
       
        yield return Fade(0f, 1f, fadeInTime);

      
        if (showDuration > 0f)
            yield return new WaitForSeconds(showDuration);

      
        yield return Fade(1f, 0f, fadeOutTime);

       
        cg.alpha = 0f;
    }

    IEnumerator Fade(float from, float to, float time)
    {
        if (time <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / time);
            cg.alpha = Mathf.Lerp(from, to, lerp);
            yield return null;
        }
        cg.alpha = to;
    }
}
