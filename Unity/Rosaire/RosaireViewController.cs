using UnityEngine;
using Unity.XR.CoreUtils;

public class RosaireViewController : MonoBehaviour
{
    [Header("텔레포트 관련")]
    public Transform TeleportPointC;
    public XROrigin xrOrigin;

    [Header("숨길 오브젝트들 (제단 관련)")]
    public GameObject AltarWindow;
    public GameObject AltarGlass;

    [Header("보여줄 오브젝트들")]
    public GameObject LoraPlane;
    public Transform CutoutFrameParent;

    [Header("포털/핫스팟 오브젝트들")]
    public GameObject PortalB;      // 포탈B에 드래그
    public GameObject HotspotB;     // 핫스팟 B

    [Header("버튼 UI")]
    public GameObject TeleportUI;   // 버튼 UI (나중에 숨김)

    // 사용자가 UI 버튼을 눌렀을 때 실행되는 함수

    public void OnViewButtonPressed()
    {
        Debug.Log("-> OnViewButtonPressed() 실행됨!");

        // 제단 오브젝트 숨기기
        if (AltarWindow != null) AltarWindow.SetActive(false);
        if (AltarGlass != null) AltarGlass.SetActive(false);

        // LoRA 이미지 표시
        if (LoraPlane != null) LoraPlane.SetActive(true);

        // 컷아웃 프레임 표시
        if (CutoutFrameParent != null)
        {
            foreach (Transform f in CutoutFrameParent)
                f.gameObject.SetActive(true);
        }

        // 포털 B, 핫스팟 B 비활성화
        if (PortalB != null) PortalB.SetActive(false);
        if (HotspotB != null) HotspotB.SetActive(false);

        // 버튼 UI 숨김
        if (TeleportUI != null) TeleportUI.SetActive(false);

        Debug.Log("포털B, 핫스팟B 비활성화");
    }
}
