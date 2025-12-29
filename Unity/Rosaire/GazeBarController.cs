using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GazeBarController : MonoBehaviour
{
    [Header("Gaze UI")]
    public GameObject gazeFrame;      
    public Image gazeFill;            

    [Header("Main Image")]
    public Image mainImage;           
    public Sprite mainImageAfterFill;

    [Header("Button")]
    public GameObject targetButton;  
    
    [Header("Timing")]
    public float startDelay = 5f;     
    public float fillTime = 3f;       
    
    [Header("Audio")]
    public AudioSource narrationAudio; 
    

    void Start()
    {
        if (gazeFrame != null) gazeFrame.SetActive(false);
        if (gazeFill != null) gazeFill.gameObject.SetActive(false);

        if (targetButton != null) targetButton.SetActive(false);

        if (mainImage != null) mainImage.gameObject.SetActive(false);

        StartCoroutine(GazeFlowRoutine());
    }


    IEnumerator GazeFlowRoutine()
    {
     
        yield return new WaitForSeconds(startDelay);

        if (mainImage != null)
            mainImage.gameObject.SetActive(true);

        
        if (narrationAudio != null)
            narrationAudio.Play();

        
        if (gazeFrame != null) gazeFrame.SetActive(true);
        if (gazeFill != null) gazeFill.gameObject.SetActive(true);

        
        gazeFill.fillAmount = 0f;

        float t = 0f;

        
        while (t < fillTime)
        {
            t += Time.deltaTime;
            gazeFill.fillAmount = t / fillTime;
            yield return null;
        }

        gazeFill.fillAmount = 1f;

        
        if (gazeFrame != null) gazeFrame.SetActive(false);
        if (gazeFill != null) gazeFill.gameObject.SetActive(false);

        
        if (mainImage != null && mainImageAfterFill != null)
            mainImage.sprite = mainImageAfterFill;


        if (targetButton != null)
            targetButton.SetActive(true);
    }
}
