using UnityEngine;
using Unity.XR.CoreUtils; // XROrigin

public class TeleportHotspot : MonoBehaviour
{
    public Transform teleportPoint;   // 이동할 바닥 위치
    public XROrigin xrOrigin;        // XR Origin

    public GameObject uiToShowOnArrive; // 선택: 도착 후 UI 켜기

    public void Teleport()
    {
        Debug.Log("Teleport() 실행됨");   // 1단계

        if (teleportPoint == null) Debug.Log("⚠ teleportPoint == null");
        if (xrOrigin == null) Debug.Log("⚠ xrOrigin == null");

        Debug.Log("teleportPoint = " + teleportPoint);
        Debug.Log("xrOrigin = " + xrOrigin);

        Vector3 offset = xrOrigin.CameraInOriginSpacePos;
        Debug.Log("offset = " + offset);

        Vector3 dest = teleportPoint.position - offset;
        Debug.Log("dest = " + dest);

        xrOrigin.transform.position = dest;

        if (uiToShowOnArrive != null)
            uiToShowOnArrive.SetActive(true);

        Debug.Log("텔레포트 완료");
    }

}
