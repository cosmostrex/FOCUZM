using UnityEngine;
using UnityEngine.InputSystem;

public class BrushTipContactDetector : MonoBehaviour
{
    [Header("붓 끝(끝점)")]
    public Transform tip;

    [Header("감지 반경/레이어")]
    public float radius = 0.01f;
    public LayerMask paintLayer = ~0;

    [Header("첫 접촉 시 알릴 대상 (PickerPipeline)")]
    [Tooltip("같은 씬에 있는 PickerPipeline을 드래그해서 넣기")]
    public ColorPicker.PickerPipeline pickerPipeline;

    [Header("버튼 입력 (트리거, Grip 등)")]
    public InputActionReference paintAction;  

    private bool fired = false;

    void Reset()
    {
        tip = transform;
    }

    void Start()
    {
        if (paintAction != null && paintAction.action != null)
            paintAction.action.Enable();
    }

    void OnDestroy()
    {
        if (paintAction != null && paintAction.action != null)
            paintAction.action.Disable();
    }

    void Update()
    {
        if (fired || tip == null || pickerPipeline == null) return;

       
        if (paintAction == null || paintAction.action == null || !paintAction.action.IsPressed())
            return;

       
        var hits = Physics.OverlapSphere(
            tip.position,
            radius,
            paintLayer,
            QueryTriggerInteraction.Collide
        );

        if (hits != null && hits.Length > 0)
        {
           
            pickerPipeline.NotifyPaintStroke();
            fired = true;
            enabled = false;

            Debug.Log("[BrushTipContactDetector] 첫 페인트 감지 → NotifyPaintStroke 호출");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (tip == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(tip.position, radius);
    }
}
