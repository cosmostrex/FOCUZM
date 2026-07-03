using UnityEngine;
using System.Collections.Generic;

public class TeleportManager : MonoBehaviour
{
    [Header("Dependencies")]
    public GazeRecorder gazeRecorder;

    [Header("Wall Object References")]
    [Tooltip("왼쪽(인물화) 벽 오브젝트")]
    public GameObject wallLeft;
    [Tooltip("정면(풍경화) 벽 오브젝트")]
    public GameObject wallFront;
    [Tooltip("오른쪽(정물화) 벽 오브젝트")]
    public GameObject wallRight;

    // 벽 구역 구분을 위한 열거형
    public enum WallType { Left, Front, Right }

    void Start()
    {
        // 시작 시 기본적으로 왼쪽 벽 환경으로 초기화
        SetGazeEnvironment(WallType.Left);
    }

    /// <summary>
    /// 지정된 벽 타입에 맞춰 시선 기록을 초기화하고 콜라이더를 활성화합니다.
    /// </summary>
    public void SetGazeEnvironment(WallType targetWall)
    {
        if (gazeRecorder != null)
        {
            // 새로운 구역으로 이동했으므로 기존 시선 체류 시간 기록을 초기화
            gazeRecorder.ResetGazeTimes();
        }

        // 해당 구역의 벽 콜라이더만 활성화
        SetWallColliders(targetWall);

        Debug.Log($"[TeleportManager] Gaze Environment set for {targetWall} wall.");
    }

    /// <summary>
    /// 모든 벽의 콜라이더를 일단 끄고, 현재 액티브된 벽의 콜라이더만 켭니다.
    /// </summary>
    private void SetWallColliders(WallType activeWall)
    {
        SetCollidersRecursively(wallLeft, false);
        SetCollidersRecursively(wallFront, false);
        SetCollidersRecursively(wallRight, false);

        // 선택된 벽 구역의 콜라이더 및 자식 오브젝트의 콜라이더 활성화
        if (activeWall == WallType.Left) SetCollidersRecursively(wallLeft, true);
        else if (activeWall == WallType.Front) SetCollidersRecursively(wallFront, true);
        else if (activeWall == WallType.Right) SetCollidersRecursively(wallRight, true);
    }

    /// <summary>
    /// 대상 오브젝트 및 그 하위 자식(Child) 오브젝트들의 모든 Collider 컴포넌트를 찾아 활성화/비활성화합니다.
    /// </summary>
    private void SetCollidersRecursively(GameObject obj, bool isEnabled)
    {
        if (obj == null) return;

        // 현재 오브젝트에 붙은 모든 콜라이더 제어
        Collider[] colliders = obj.GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = isEnabled;
        }

        // 자식 오브젝트들을 순회하며 재귀적으로 콜라이더 제어
        foreach (Transform child in obj.transform)
        {
            SetCollidersRecursively(child.gameObject, isEnabled);
        }
    }
}
