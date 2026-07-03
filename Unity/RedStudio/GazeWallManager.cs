using UnityEngine;
using UnityEngine.Events;

public enum WallType { Portrait, Landscape, StillLife }

public class GazeWallManager : MonoBehaviour
{
    [Header("카메라 & 감지 설정")]
    public Camera mainCamera;
    public LayerMask gazeLayer;
    public float maxDistance = 15f;

    [Header("측정 설정")]
    public float checkInterval = 0.1f;   
    public float gazeDuration = 20f;     
    public float minFocusTime = 1.0f;    
    public bool autoStop = false;       

    [Header("결과 이벤트")]
    public UnityEvent<WallType> OnWallDecided;

    private float portraitTime, landscapeTime, stillLifeTime;
    private bool isTracking;
    private float timer;
    private float elapsed;

    private string currentTag;
    private float focusTimer;

    public void StartGazeMode()
    {
        portraitTime = landscapeTime = stillLifeTime = 0;
        timer = 0f;
        elapsed = 0f;
        currentTag = null;
        focusTimer = 0f;
        isTracking = true;
        Debug.Log("응시 측정 시작");
    }

    void Update()
    {
        if (!isTracking || mainCamera == null) return;

        elapsed += Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;

            var ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, gazeLayer))
            {
                string hitTag = hit.collider.tag;

                if (hitTag == currentTag)
                {
                    focusTimer += checkInterval;

                    if (focusTimer >= minFocusTime)
                    {
                        if (hit.collider.CompareTag("PortraitWall")) portraitTime += checkInterval;
                        else if (hit.collider.CompareTag("LandscapeWall")) landscapeTime += checkInterval;
                        else if (hit.collider.CompareTag("StillLifeWall")) stillLifeTime += checkInterval;
                    }
                }
                else
                {
                    currentTag = hitTag;
                    focusTimer = 0f;
                }
            }
            else
            {
                currentTag = null;
                focusTimer = 0f;
            }
        }

        if (autoStop && elapsed >= gazeDuration)
        {
            StopAndDecide();
        }
    }

    public void StopAndDecide()
    {
        if (!isTracking) return;
        isTracking = false;

        WallType result = GetLongestGaze();
        Debug.Log($"가장 오래 본 벽: {result}");
        OnWallDecided?.Invoke(result);
    }

    public WallType GetLongestGaze()
    {
        if (portraitTime > landscapeTime && portraitTime > stillLifeTime) return WallType.Portrait;
        if (landscapeTime > stillLifeTime) return WallType.Landscape;
        return WallType.StillLife;
    }
}
