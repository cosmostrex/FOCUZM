using UnityEngine;
using Unity.XR.CoreUtils;

public class RosaireViewController : MonoBehaviour
{
    [Header("텔레포트 관련")]
    public Transform teleportPointC;
    public XROrigin xrOrigin;

    [Header("숨길 오브젝트들 (제단 관련)")]
    public GameObject altarWindow;   // 제단 창
    public GameObject altarGlass;    // 제단 납창

    [Header("보여줄 오브젝트들")]
    public GameObject loraPlane;
    public GameObject cutoutFrameParent;

    [Header("버튼 UI (감상 버튼)")]
    public GameObject teleportUI;

    public void OnViewButtonPressed()
    {
        // 1. 텔레포트 UI 숨기기
        if (teleportUI != null)
            teleportUI.SetActive(false);

        // 2. 제단 관련 오브젝트 숨기기
        if (altarWindow != null)
            altarWindow.SetActive(false);

        if (altarGlass != null)
            altarGlass.SetActive(false);

        // 3. LoRA plane & 컷아웃 프레임 활성화
        if (loraPlane != null)
            loraPlane.SetActive(true);

        if (cutoutFrameParent != null)
            cutoutFrameParent.SetActive(true);

        // 4. 감상 위치로 텔레포트
        TeleportToViewPoint();
    }

    private void TeleportToViewPoint()
    {
        if (teleportPointC == null || xrOrigin == null)
        {
            Debug.LogWarning("텔레포트 설정이 비어 있음");
            return;
        }

        Vector3 offset = xrOrigin.CameraInOriginSpacePos;
        Vector3 dest = teleportPointC.position - offset;

        xrOrigin.transform.position = dest;
    }
}
