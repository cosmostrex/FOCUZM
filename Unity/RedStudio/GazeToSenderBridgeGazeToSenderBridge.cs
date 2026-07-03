using UnityEngine;

public class GazeToSenderBridge : MonoBehaviour
{
    public GazeWallManager gaze;
    public AIRequestSenderPro sender;

    void OnEnable() { gaze?.OnWallDecided.AddListener(OnDecided); }
    void OnDisable() { gaze?.OnWallDecided.RemoveListener(OnDecided); }

    void OnDecided(WallType t)
    {
        sender?.SetPrimaryType(t);
        Debug.Log($"🚀 PrimaryType <- {t}");
    }
}
