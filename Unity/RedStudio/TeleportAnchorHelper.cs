using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportAnchorHelper : MonoBehaviour
{
    [Header("Dependencies")]
    [Tooltip("Hierarchy에서 TeleportManager 오브젝트를 연결하세요.")]
    public TeleportManager teleportManager;

    [Tooltip("이 앵커가 활성화될 타겟 타입을 설정하세요.")]
    public TeleportManager.WallType wallType;

    [Header("Anchor Component")]
    [Tooltip("이 오브젝트에 있는 Teleportation Anchor 컴포넌트를 직접 연결하세요.")]
    public TeleportationAnchor anchor;

    void Start()
    {
        // 컴포넌트 연결 누락 확인 및 경고
        if (anchor == null)
        {
            Debug.LogError("TeleportAnchorHelper: Teleportation Anchor 컴포넌트가 Inspector에 설정되지 않았습니다.");
        }
        if (teleportManager == null)
        {
            Debug.LogError("TeleportAnchorHelper: TeleportManager 오브젝트가 연결되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// 텔레포트가 완료되거나 상호작용이 일어날 때 호출하여 해당 구역의 환경을 세팅합니다.
    /// XR Base Interactable의 On Teleport Trigger 등 이벤트에 연결하여 사용하기 좋습니다.
    /// </summary>
    public void TriggerGazeEnvironmentSet()
    {
        if (teleportManager == null)
        {
            Debug.LogError("TeleportManager가 연결되지 않아 환경 세팅을 진행할 수 없습니다.");
            return;
        }

        teleportManager.SetGazeEnvironment(wallType);
        Debug.Log($"[AnchorHelper] 텔레포트 이벤트 발생. 환경 세팅({wallType}) 요청.");
    }
}
