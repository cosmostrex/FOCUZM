using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractable))]
public class ArtworkHoverFx : MonoBehaviour
{
    [Header("하이라이트 (Highlight Plus 등의 효과)")]
    [Tooltip("Highlight Effect 컴포넌트를 드래그해서 넣기")]
    public Behaviour highlightEffect;   // <- 컴포넌트 연결용

    [Header("각 작품 정보 힌트 UI (World Space)")]
    public GameObject hintUi;

    private XRBaseInteractable it;

    void Awake()
    {
        it = GetComponent<XRBaseInteractable>();

        // 이벤트 리스너 등록 (마우스/컨트롤러 호버 대응)
        it.hoverEntered.AddListener(OnHoverEnter);
        it.hoverExited.AddListener(OnHoverExit);

        // 시작 시에는 항상 비활성화
        if (highlightEffect != null)
            highlightEffect.enabled = false;

        if (hintUi != null)
            hintUi.SetActive(false);
    }

    void OnDestroy()
    {
        if (it != null)
        {
            it.hoverEntered.RemoveListener(OnHoverEnter);
            it.hoverExited.RemoveListener(OnHoverExit);
        }
    }

    void OnHoverEnter(HoverEnterEventArgs _)
    {
        SetHover(true);
    }

    void OnHoverExit(HoverExitEventArgs _)
    {
        SetHover(false);
    }

    void SetHover(bool on)
    {
        // 하이라이트 효과 On/Off
        if (highlightEffect != null)
            highlightEffect.enabled = on;

        // 힌트 UI On/Off
        if (hintUi != null)
            hintUi.SetActive(on);
    }
}
