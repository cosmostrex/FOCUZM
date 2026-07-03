using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneBootstrapper : MonoBehaviour
{
    [Header("시작 락 (ViewSpotLock 연결)")]
    public ViewSpotLock startLock;

    [Header("선택/응시 데이터 관리")]
    public SelectionAggregator aggregator;

    [Header("XR 컨트롤러 레이 (시작 시 비활성화)")]
    public XRRayInteractor leftRay;
    public XRRayInteractor rightRay;
    public XRInteractorLineVisual leftLine;
    public XRInteractorLineVisual rightLine;

    [Header("테스트 모드 설정")]
    [Tooltip("테스트 모드일 경우 G키로 즉시 잠금 해제")]
    public bool testMode = true;

    void Start()
    {
        
        aggregator?.Begin();

        
        if (leftRay) leftRay.enabled = false;
        if (rightRay) rightRay.enabled = false;
        if (leftLine) leftLine.enabled = false;
        if (rightLine) rightLine.enabled = false;

        
        if (testMode)
        {
            startLock?.Unlock();
            Debug.Log("▶ Boot: StartLock OFF (Test Mode) — 바로 응시 가능");
        }
        else
        {
            startLock?.Lock();
            Debug.Log("▶ Boot: StartLock ON, Rays OFF, Selection reset");
        }
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            startLock?.Unlock();
            Debug.Log("G키 입력: StartLock 해제, 응시 시작 가능");
        }
    }
}
