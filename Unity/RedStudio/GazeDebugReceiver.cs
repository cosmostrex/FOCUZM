using UnityEngine;

public class GazeDebugReceiver : MonoBehaviour
{
    /// <summary>
    /// 시선(Gaze) 처리에 의해 특정 벽이 결정되었을 때 호출되는 메서드입니다.
    /// </summary>
    public void OnWallDecided(WallType t)   
    {
        Debug.Log($"[Gaze] 최종 결정된 벽 종류: {t}");
    }
}
