using UnityEngine;
using System.Collections;

public class RedRoomUIDirector : MonoBehaviour
{
    [Header("=== 초기 안내 UI ===")]
    public UIGuidedFade lookAroundUI;     
    public float startDelay = 3f;         

    [Header("=== 응시(시선) 안내 UI ===")]
    public UIGuidedFade selectArtworkUI;  
    public float gazeNodeDelay = 5f;      
    public float betweenGazeTextsDelay = 0.5f;

    [Header("=== 캔버스 안내 UI ===")]
    public UIGuidedFade pickColorUI;      
    public UIGuidedFade influenceUI;      // 문맥상 색상 선택 후 영향을 주는 연출 UI
    public float canvasNodeDelay = 4f;    

    [Header("뷰스팟 잠금 설정")]
    public ViewSpotLock startLock;        
    public ViewSpotLock gazeLock;         
    public AudioSource narration;       // 나레이션 오디오

    
    private bool startedGazeGuide = false;
    private bool startedCanvasGuide = false;
    private bool hasVisitedGaze = false; 
    private bool wasAtStartLock = false;
    private bool wasAtGazeLock = false;

    void Start()
    {
        // 시작 시 주변을 둘러보라는 안내 루틴 시작
        StartCoroutine(StartLookAroundRoutine());
    }

    IEnumerator StartLookAroundRoutine()
    {
        yield return new WaitForSeconds(startDelay);
        lookAroundUI?.Play();
    }

    void Update()
    {
        // 현재 플레이어가 startLock 위치나 gazeLock 위치에 도달했는지 확인
        bool nowAtStart = IsAtLock(startLock);
        bool nowAtGaze = IsAtLock(gazeLock);

        // gazeLock 위치에 처음 도달했을 때 응시 안내 루틴 실행
        if (!startedGazeGuide && nowAtGaze && !wasAtGazeLock)
        {
            startedGazeGuide = true;
            hasVisitedGaze = true;
            StartCoroutine(GazeModeRoutine());
        }
        
        // gazeLock을 방문한 적이 있고, 다시 startLock(처음 지점)으로 돌아왔을 때 캔버스 안내 루틴 실행
        if (hasVisitedGaze && !startedCanvasGuide && nowAtStart && !wasAtStartLock)
        {
            startedCanvasGuide = true;
            StartCoroutine(CanvasModeRoutine());
        }

        // 이전 프레임의 위치 상태 업데이트
        wasAtStartLock = nowAtStart;
        wasAtGazeLock = nowAtGaze;
    }

    // 플레이어(xrOrigin)와 목표 지점(lockPoint) 사이의 거리가 0.05m(5cm) 이내인지 체크하는 함수
    bool IsAtLock(ViewSpotLock lockComp)
    {
        if (!lockComp || !lockComp.xrOrigin || !lockComp.lockPoint)
            return false;

        return Vector3.Distance(
            lockComp.xrOrigin.transform.position,
            lockComp.lockPoint.position
        ) < 0.05f;
    }

   

    IEnumerator GazeModeRoutine()
    {
        // 지정된 딜레이만큼 대기
        yield return new WaitForSeconds(gazeNodeDelay);

        // 작품 선택 UI 재생
        if (selectArtworkUI)
        {
            selectArtworkUI.Play();

            // UI가 페이드인 + 유지 + 페이드아웃 되는 총 시간만큼 대기
            float total = selectArtworkUI.fadeInTime
                        + selectArtworkUI.showDuration
                        + selectArtworkUI.fadeOutTime;
            if (total > 0f)
                yield return new WaitForSeconds(total);
        }

        // UI 사이의 간격 딜레이 대기
        if (betweenGazeTextsDelay > 0f)
            yield return new WaitForSeconds(betweenGazeTextsDelay);

        // 시선 영향/연출 UI 재생
        if (influenceUI)
        {
            influenceUI.Play();
        }
    }

  

    IEnumerator CanvasModeRoutine()
    {
        // 지정된 딜레이만큼 대기
        yield return new WaitForSeconds(canvasNodeDelay);

        // 색상 선택 UI 재생
        if (pickColorUI)
        {
            pickColorUI.Play();

            // UI의 총 연출 시간만큼 대기
            float total = pickColorUI.fadeInTime
                        + pickColorUI.showDuration
                        + pickColorUI.fadeOutTime;
            if (total > 0f)
                yield return new WaitForSeconds(total);
        }

        // 연출 UI 재생
        if (influenceUI)
        {
            influenceUI.Play();
        }
    }
}
