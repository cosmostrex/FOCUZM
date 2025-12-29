using UnityEngine;
using UnityEngine.SceneManagement;  

[System.Serializable]
public class IntroImageData
{
    public CanvasGroup image;        
    public AudioSource
    public float fadeIn = 1f;       
    public float hold = 1.5f;        
    public float fadeOut = 1f;   
}

public class IntroImageSequence : MonoBehaviour
{
    [Header("Intro Images (������� �ֱ�!)")]
    public IntroImageData[] images;

    [Header("��ü ���� ����� (��)")]
    public float startDelay = 1f;

    [Header("�̹��� ���� ����� (��)")]
    public float delayBetweenImages = 0.5f;

    [Header("�����̼� ���� �� ���� ������ �Ѿ��")]
    public string nextSceneName = "Intro";   
    public float delayAfterNarration = 3f;  

    private void Start()
    {
        
        foreach (var img in images)
        {
            if (img.image != null)
            {
                img.image.alpha = 0f;
                img.image.gameObject.SetActive(false);
            }

            if (img.narration != null)
            {
                img.narration.Stop();
            }
        }

        StartCoroutine(StartSequence());
    }

    private System.Collections.IEnumerator StartSequence()
    {
        
        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        
        for (int i = 0; i < images.Length; i++)
        {
            var data = images[i];

            if (data.image == null)
                continue;

            
            data.image.gameObject.SetActive(true);

            
            if (data.fadeIn > 0f)
                yield return StartCoroutine(Fade(data.image, 0f, 1f, data.fadeIn));
            else
                data.image.alpha = 1f;

            
            if (data.narration != null)
                data.narration.Play();

        
            if (data.hold > 0f)
                yield return new WaitForSeconds(data.hold);

            
            if (data.fadeOut > 0f)
                yield return StartCoroutine(Fade(data.image, 1f, 0f, data.fadeOut));
            else
                data.image.alpha = 0f;

            
            data.image.gameObject.SetActive(false);

            
            if (i < images.Length - 1 && delayBetweenImages > 0f)
                yield return new WaitForSeconds(delayBetweenImages);
        }

        
        bool anyPlaying;
        do
        {
            anyPlaying = false;
            foreach (var d in images)
            {
                if (d.narration != null && d.narration.isPlaying)
                {
                    anyPlaying = true;
                    break;
                }
            }
            if (anyPlaying)
                yield return null;
        } while (anyPlaying);

        
        if (delayAfterNarration > 0f)
            yield return new WaitForSeconds(delayAfterNarration);

        
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        
    }

    private System.Collections.IEnumerator Fade(CanvasGroup cg, float start, float end, float duration)
    {
        float t = 0f;

        if (duration <= 0f)
        {
            cg.alpha = end;
            yield break;
        }

        while (t < duration)
        {
            t += Time.deltaTime;  
            cg.alpha = Mathf.Lerp(start, end, t / duration);
            yield return null;
        }

        cg.alpha = end;
    }
}
