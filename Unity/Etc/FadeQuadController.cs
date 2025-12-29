using UnityEngine;
using System.Collections;

public class FadeQuadController : MonoBehaviour
{
    public static FadeQuadController Instance;

    public float fadeDuration = 1f;
    private Material fadeMat;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        fadeMat = GetComponent<MeshRenderer>().material;
        SetAlpha(1f);  
    }

    
    void SetAlpha(float a)
    {
        Color c = fadeMat.color;
        c.a = a;
        fadeMat.color = c;
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(1f, 0f, t / fadeDuration);
            SetAlpha(v);
            yield return null;
        }
        SetAlpha(0f);
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(0f, 1f, t / fadeDuration);
            SetAlpha(v);
            yield return null;
        }
        SetAlpha(1f);
    }
}
