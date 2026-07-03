using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSfx : MonoBehaviour
{
    [Header("효과음을 재생할 AudioSource")]
    public AudioSource audioSource;      

    [Header("버튼 클릭 시 재생할 클립")]
    public AudioClip clickClip;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

      
        _button.onClick.AddListener(PlayClickSound);
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (audioSource == null || clickClip == null)
            return;

        audioSource.PlayOneShot(clickClip);
    }
}
