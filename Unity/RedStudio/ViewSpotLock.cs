using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class ViewSpotLock : MonoBehaviour
{
    [Header("필수 연결")]
    public XROrigin xrOrigin;
    public Transform lockPoint;           

    [Header("이동 제한")]
    public LocomotionSystem locomotionSystem;
    public TeleportationProvider teleportProvider;
    public ContinuousMoveProviderBase moveProvider;
    public ContinuousTurnProviderBase turnProvider;

    [Header("옵션")]
    public bool lockRotation = true;      
    public bool lockPosition = true;     
    public bool ensureKinematicRigidbody = true;

    [Header("Clamp 옵션 (원한다면 사용)")]
    public bool clampWithinBox = false;
    public Transform clampCenter;
    public Vector2 xzHalfRange = new Vector2(0.3f, 0.0f);

    bool isLocked = false;
    Rigidbody rb;

    void Awake()
    {
        if (!xrOrigin) xrOrigin = FindObjectOfType<XROrigin>();

        if (!locomotionSystem) locomotionSystem = FindObjectOfType<LocomotionSystem>();
        if (!teleportProvider) teleportProvider = FindObjectOfType<TeleportationProvider>();
        if (!moveProvider) moveProvider = FindObjectOfType<ContinuousMoveProviderBase>();
        if (!turnProvider) turnProvider = FindObjectOfType<ContinuousTurnProviderBase>();

        if (ensureKinematicRigidbody && xrOrigin)
        {
            rb = xrOrigin.GetComponent<Rigidbody>();
            if (!rb) rb = xrOrigin.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void LateUpdate()
    {
        if (!isLocked || !xrOrigin || !lockPoint) return;

       
        if (!clampWithinBox)
        {
            if (lockPosition)
                xrOrigin.transform.position = lockPoint.position;

            if (lockRotation)
                xrOrigin.transform.rotation = lockPoint.rotation;

            return;
        }

     
        Vector3 camPos = xrOrigin.Camera.transform.position;
        Vector3 center = clampCenter ? clampCenter.position : lockPoint.position;

        float x = Mathf.Clamp(camPos.x, center.x - xzHalfRange.x, center.x + xzHalfRange.x);
        float z = Mathf.Clamp(camPos.z, center.z - xzHalfRange.y, center.z + xzHalfRange.y);

        xrOrigin.MoveCameraToWorldLocation(new Vector3(x, camPos.y, z));

        if (lockRotation)
            xrOrigin.transform.rotation = lockPoint.rotation;
    }

    public void Lock()
    {
        if (isLocked) return;
        isLocked = true;

       
        if (lockPosition)
            xrOrigin.transform.position = lockPoint.position;

        if (lockRotation)
            xrOrigin.transform.rotation = lockPoint.rotation;

     
        if (locomotionSystem) locomotionSystem.enabled = false;
        if (teleportProvider) teleportProvider.enabled = false;
        if (moveProvider) moveProvider.enabled = false;
        if (turnProvider) turnProvider.enabled = false;

        Debug.Log("ViewSpotLock : ON / 완전 고정형");
    }

    public void Unlock()
    {
        if (!isLocked) return;
        isLocked = false;

        if (locomotionSystem) locomotionSystem.enabled = true;
        if (teleportProvider) teleportProvider.enabled = true;
        if (moveProvider) moveProvider.enabled = true;
        if (turnProvider) turnProvider.enabled = true;

        Debug.Log("ViewSpotLock : OFF");
    }
}
