using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractable))]
public class ArtworkSelectSfx : MonoBehaviour
{
    [Header("효과음을 재생할 AudioSource")]
    public AudioSource audioSource;     

    [Header("그림 선택 시 재생할 클립")]
    public AudioClip openClip;         
    XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectEntered.AddListener(OnSelected);
    }

    void OnDestroy()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelected);
    }

    void OnSelected(SelectEnterEventArgs args)
    {
        if (audioSource != null && openClip != null)
        {
            audioSource.PlayOneShot(openClip);
        }
    }
}
