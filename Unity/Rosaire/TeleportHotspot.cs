using UnityEngine;
using Unity.XR.CoreUtils;

public class TeleportHotspot : MonoBehaviour
{
    [Header("텔레포트 도착 위치")]
    public Transform teleportPoint;    // XR Origin이 도착할 위치

    [Header("도착했을 때 바라볼 방향(Transform의 Rotation 사용)")]
    public Transform lookDirectionTarget;   // XR Origin이 바라볼 회전

    [Header("XR Origin")]
    public XROrigin xrOrigin;

    [Header("도착 후 켜줄 UI (선택)")]
    public GameObject uiToShowOnArrive;

    public void Teleport()
    {
        if (xrOrigin == null || teleportPoint == null)
        {
            Debug.LogWarning("TeleportHotspot: XR Origin 또는 TeleportPoint가 비어있음");
            return;
        }

        // XR Origin의 카메라 오프셋 보정용
        Vector3 offset = xrOrigin.CameraInOriginSpacePos;

        // 위치 이동
        Vector3 destination = teleportPoint.position - offset;
        xrOrigin.transform.position = destination;

        // 회전 적용
        if (lookDirectionTarget != null)
        {
            xrOrigin.transform.rotation = lookDirectionTarget.rotation;
        }

        // UI 활성화
        if (uiToShowOnArrive != null)
            uiToShowOnArrive.SetActive(true);

        Debug.Log("텔레포트 완료 + 바라볼 방향까지 적용됨");
    }
}
