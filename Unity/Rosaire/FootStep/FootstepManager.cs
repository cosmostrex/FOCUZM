// 풋스텝 매니저의 경우 로사리오 전체 씬을 관장함.
// 풋스텝 매니저는 씬 매니저의 역할을 하기도 함(추후 추가 예정…)

using UnityEngine;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class FootstepManager : MonoBehaviour
{
    [Header("발자국 그룹 (순서대로)")]
    public GameObject[] footstepGroups;

    [Header("XR 레퍼런스")]
    public XROrigin xrOrigin;                    // XR Origin
    public Transform cameraTransform;            // Main Camera (XR Origin 안)
    public LocomotionSystem locomotionSystem;
    public ContinuousMoveProviderBase moveProvider;
    public ContinuousTurnProviderBase turnProvider;
    public TeleportationProvider teleportProvider;
    [Tooltip("에디터에서 키보드로 움직일 수 있는 XR Device Simulator가 있다면 연결해서 잠가주세요")]
    public Behaviour xrDeviceSimulator;

    [Header("타이밍")]
    public float headFreeSeconds = 5f;           // foot3 직후 머리만 자유인 시간
    public float rotateDuration = 2f;            // 최종 회전 시간

    [Header("공통 시선(인스펙터에서 지정)")]
    public bool useTransformForCommonView = true;
    public Transform commonViewTransform;        // 이 트랜스폼의 Yaw를 '공통 시선'으로 사용
    public Vector3 commonViewEuler = new Vector3(0, 0, 0); // 트랜스폼이 없을 때 사용할 오일러(Y만 보통 사용)

    [Header("최종 회전")]
    public float finalYawDelta = 180f;           // 공통 시선에서 추가로 돌릴 Yaw(예: 180도)

    // 내부
    private int currentGroupIndex = 0;
    private bool isProcessing = false;

    void Start()
    {
        if (!xrOrigin) xrOrigin = FindObjectOfType<XROrigin>();
        if (!cameraTransform)
        {
            if (xrOrigin && xrOrigin.Camera) cameraTransform = xrOrigin.Camera.transform;
            else if (Camera.main) cameraTransform = Camera.main.transform;
        }

        foreach (var g in footstepGroups) if (g) g.SetActive(false);
        if (footstepGroups.Length > 0 && footstepGroups[0]) footstepGroups[0].SetActive(true);
    }

    public void ShowNextFootstepGroup()
    {
        if (!isProcessing) StartCoroutine(NextGroup());
    }

    private IEnumerator NextGroup()
    {
        isProcessing = true;
        currentGroupIndex++;
        if (currentGroupIndex < footstepGroups.Length)
        {
            yield return null;
            footstepGroups[currentGroupIndex].SetActive(true);
        }
        isProcessing = false;
    }

    // foot3 마지막이 isFinalEventTrigger=true로 들어오면 이걸 호출
    public void StartFinalEvent(float _ = -1f)
    {
        if (!isProcessing) StartCoroutine(FinalEventRoutine());
    }

    private IEnumerator FinalEventRoutine()
    {
        isProcessing = true;

        SetLocomotion(move: false, turn: false, teleport: false, sim: false);

        if (headFreeSeconds > 0f)
            yield return FreezePositionForSeconds(headFreeSeconds);

        float targetCommonYaw = GetCommonViewYaw();
        SnapYawTo(targetCommonYaw);

        yield return RotateYawToAngle(Mathf.Repeat(targetCommonYaw + finalYawDelta, 360f), rotateDuration);

        SetLocomotion(move: false, turn: false, teleport: false, sim: false);

        isProcessing = false;
    }

    private void SetLocomotion(bool move, bool turn, bool teleport, bool sim)
    {
        if (locomotionSystem) locomotionSystem.enabled = true;
        if (moveProvider) moveProvider.enabled = move;
        if (turnProvider) turnProvider.enabled = turn;
        if (teleportProvider) teleportProvider.enabled = teleport;
        if (xrDeviceSimulator) xrDeviceSimulator.enabled = sim;
    }

    private IEnumerator FreezePositionForSeconds(float seconds)
    {
        if (!xrOrigin) { yield return new WaitForSeconds(seconds); yield break; }

        Transform origin = xrOrigin.transform;
        Vector3 lockedPos = origin.position;

        var rb = origin.GetComponent<Rigidbody>();
        bool rbHad = rb != null;
        bool rbPrevKinematic = false;
        if (rbHad) { rbPrevKinematic = rb.isKinematic; rb.isKinematic = true; rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        float t = 0f;
        while (t < seconds)
        {

            origin.position = lockedPos;
            t += Time.deltaTime;
            yield return null;
        }

        if (rbHad) rb.isKinematic = rbPrevKinematic;
    }

    private float GetCommonViewYaw()
    {
        if (useTransformForCommonView && commonViewTransform)
        {
            Vector3 f = Vector3.ProjectOnPlane(commonViewTransform.forward, Vector3.up).normalized;
            if (f.sqrMagnitude > 1e-6f)
                return Quaternion.LookRotation(f, Vector3.up).eulerAngles.y;
        }
        return commonViewEuler.y;
    }

    private void SnapYawTo(float targetYaw)
    {
        if (!xrOrigin || !cameraTransform) return;

        float currentYaw = cameraTransform.eulerAngles.y;
        float deltaYaw = Mathf.DeltaAngle(currentYaw, targetYaw);
        xrOrigin.transform.RotateAround(cameraTransform.position, Vector3.up, deltaYaw);
    }
    
    private IEnumerator RotateYawToAngle(float targetYaw, float duration)
    {
        if (!xrOrigin || !cameraTransform) yield break;

        float startYaw = cameraTransform.eulerAngles.y;
        float t = 0f, prevYaw = startYaw;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerpYaw = Mathf.LerpAngle(startYaw, targetYaw, Mathf.Clamp01(t / duration));
            float step = Mathf.DeltaAngle(prevYaw, lerpYaw);
            xrOrigin.transform.RotateAround(cameraTransform.position, Vector3.up, step);
            prevYaw = lerpYaw;
            yield return null;
        }

        float remain = Mathf.DeltaAngle(cameraTransform.eulerAngles.y, targetYaw);
        xrOrigin.transform.RotateAround(cameraTransform.position, Vector3.up, remain);
    }
}
