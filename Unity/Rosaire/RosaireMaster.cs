using UnityEngine;
using Unity.XR.CoreUtils;

public class RosaireMaster : MonoBehaviour
{
    [Header("씬 시작 위치(텔레포트 A)")]
    public Transform teleportPointA;
    public XROrigin xrOrigin;

    [Header("초기 숨길 오브젝트들")]
    public GameObject LoraPlane;
    public Transform CutoutFrameParent;

    [Header("이동/회전 비활성화 루트")]
    public GameObject locomotionRoot;

    [Header("컷아웃에서 선택된 프레임 이름")]
    public string selectedFrameName;

    void Start()
    {
        // 0) 컷아웃 선택 가져오기
        selectedFrameName = SceneData.Instance?.selectedShapeName;

        HideAllFrames();
        ShowSelectedFrame();
        TeleportToStartPoint();

        if (locomotionRoot != null)
            locomotionRoot.SetActive(false);
    }

    void HideAllFrames()
    {
        if (CutoutFrameParent == null) return;

        foreach (Transform child in CutoutFrameParent)
            child.gameObject.SetActive(false);
    }

    void ShowSelectedFrame()
    {
        if (CutoutFrameParent == null) return;
        if (string.IsNullOrEmpty(selectedFrameName)) return;

        // 컷아웃 이름 = henricoral
        // 프레임 이름 = coral_frame, 컷아웃에 저장된 도형 이름 기반
        string keyword = selectedFrameName.Replace("henri", "");
        string targetName = keyword + "_frame";   // 최종적으로 찾을 이름

        Debug.Log($"[RosaireMaster] 프레임 찾기: {targetName}");

        Transform t = CutoutFrameParent.Find(targetName);

        if (t != null)
        {
            t.gameObject.SetActive(true);
            Debug.Log("활성화된 프레임: " + t.name);
        }
        else
        {
            Debug.LogWarning("해당 프레임 없음: " + targetName);
        }
    }

    void TeleportToStartPoint()
    {
        if (teleportPointA == null || xrOrigin == null) return;

        Vector3 offset = xrOrigin.CameraInOriginSpacePos;
        Vector3 dest = teleportPointA.position - offset;
        xrOrigin.transform.position = dest;
    }
}
