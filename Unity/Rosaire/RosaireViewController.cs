using UnityEngine;
using Unity.XR.CoreUtils;

public class RosaireViewController : MonoBehaviour
{
    [Header("텔레포트 포인트")]
    public Transform teleportPointC;
    public XROrigin xrOrigin;

    [Header("숨길 오브젝트들 (제단 창/중앙)")]
    public GameObject altarWindow;
    public GameObject altarGlass;

    [Header("보여줄 오브젝트들 (AI + 프레임 부모)")]
    public GameObject loraPlane;
    public Transform cutoutFrameParent;

    [Header("버튼 UI (제단 버튼)")]
    public GameObject viewButtonUI;

    [Header("비활성화 목록")]
    public GameObject portalB;
    public GameObject hotspotB;

    [Header("텔레포트C에서 바라볼 회전")]
    public Transform viewRotationPoint;   // 인스펙터에서 직접

    void Start()
    {
        // 시작할 때는 아무 프레임도 건들지 않음 (RosaireMaster에서 처리)
    }


    public void OnViewButtonPressed()
    {
        Debug.Log("OnViewButtonPressed() 실행됨!");

        // 제단 UI 사라지기
        if (viewButtonUI) viewButtonUI.SetActive(false);

        // 제단 창 / 납창 끄기
        if (altarWindow) altarWindow.SetActive(false);
        if (altarGlass) altarGlass.SetActive(false);

        // LoRA 보이기
        if (loraPlane) loraPlane.SetActive(true);

        // 선택된 프레임만 활성화 (전체 활성화 금지!!!)
        EnableOnlySelectedFrame();

        // 포탈 B & 핫스팟 숨기기
        if (portalB) portalB.SetActive(false);
        if (hotspotB) hotspotB.SetActive(false);

        // 포탈 C로 이동 + 바라볼 방향 적용
        TeleportToC();
    }


    //   선택된 프레임만 켜는 함수
    
    void EnableOnlySelectedFrame()
    {
        if (cutoutFrameParent == null) return;

        string selected = SceneData.Instance?.selectedShapeName;
        if (string.IsNullOrEmpty(selected)) return;

        string frameName = selected.Replace("henri", "") + "_frame";
        frameName = frameName.ToLower();

        foreach (Transform f in cutoutFrameParent)
        {
            // 이름이 정확히 매칭되는 애만 켬
            if (f.name.ToLower() == frameName)
                f.gameObject.SetActive(true);
            else
                f.gameObject.SetActive(false);
        }

        Debug.Log("제대로 활성화된 프레임: " + frameName);
    }
    
    //  텔레포트 + 원하는 시선 고정

    void TeleportToC()
    {
        if (teleportPointC == null || xrOrigin == null)
        {
            Debug.LogWarning("텔레포트C 이동 실패");
            return;
        }

        Vector3 offset = xrOrigin.CameraInOriginSpacePos;
        Vector3 dest = teleportPointC.position - offset;
        xrOrigin.transform.position = dest;

        // 언니가 지정한 방향으로 바라보게 함!
        if (viewRotationPoint != null)
        {
            Vector3 forward = viewRotationPoint.forward;
            forward.y = 0; // 고개 숙이는 건 방지
            xrOrigin.transform.forward = forward;
        }

        Debug.Log("포탈 C 위치 이동 & 시선 설정");
    }
}
