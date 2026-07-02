
using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class RedRoomConfirmHandler : MonoBehaviour
{
    [Header("뷰스팟 잠금 Lock 설정 (활성화, 비활성화 등)")]
    public ViewSpotLock[] locks;

    [Header("시선이 바라보게 할 목표 Transform (방 방향)")]
    public Transform lookTarget;

    [Header("버튼을 누른 후 몇 초 후에 회전할지")]
    public float rotateDelay = 2f;   

    [Header("컨트롤러/레이 감추기 설정")]
    public bool hideControllersOnConfirm = true;
    
    [Tooltip("왼손 XR Ray 오브젝트 (Line + Interactor 가 들어있는 오브젝트)")]
    public XRRayInteractor leftRay;
    
    [Tooltip("오른손 XR Ray 오브젝트")]
    public XRRayInteractor rightRay;

    [Tooltip("왼손 컨트롤러 모델 오브젝트 (하이라이트 기능 등 있을 때 사용)")]
    public GameObject leftControllerModel;
    
    [Tooltip("오른손 컨트롤러 모델 오브젝트")]
    public GameObject rightControllerModel;

   
    public void OnConfirmColorRotate()
    {
        // 확인 시 컨트롤러를 숨기도록 설정되어 있으면 숨김 처리
        if (hideControllersOnConfirm)
            HideControllersAndRays();

        // 기존에 돌고 있던 코루틴이 있다면 중복 실행 방지를 위해 정지
        StopAllCoroutines();

        // 지연 시간 후 회전하는 코루틴 시작
        StartCoroutine(RotateAfterDelay());
    }

    private IEnumerator RotateAfterDelay()
    {
        // 설정된 지연 시간이 있다면 대기
        if (rotateDelay > 0f)
            yield return new WaitForSeconds(rotateDelay);

        // 모든 락 해제
        foreach (var l in locks)
            if (l != null) l.Unlock();

        // XROrigin을 찾아 시선 방향을 lookTarget의 Y축 회전값에 맞춤
        var origin = FindObjectOfType<XROrigin>();
        if (origin != null && lookTarget != null)
        {
            var t = origin.transform;
            Vector3 e = t.eulerAngles;
            e.y = lookTarget.eulerAngles.y;
            t.eulerAngles = e;
        }
        else
        {
            Debug.LogWarning("[RedRoomConfirmHandler] XROrigin 또는 lookTarget 없음");
        }

        // 다시 모든 락 설정
        foreach (var l in locks)
            if (l != null) l.Lock();

        Debug.Log("[RedRoomConfirmHandler] 딜레이 후 시선 회전 완료");
    }

    private void HideControllersAndRays()
    {
        if (leftRay) leftRay.gameObject.SetActive(false);
        if (rightRay) rightRay.gameObject.SetActive(false);

        if (leftControllerModel) leftControllerModel.SetActive(false);
        if (rightControllerModel) rightControllerModel.SetActive(false);

        Debug.Log("[RedRoomConfirmHandler] 컨트롤러 + 레이 숨김");
    }
}
