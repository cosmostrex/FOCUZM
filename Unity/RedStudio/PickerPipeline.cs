
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace ColorPicker
{
    
    public class PickerPipeline : MonoBehaviour
    {
        [Header("Color Picker 입력(둘 중 하나만 연결돼도 됨)")]
        public ColorPickerVR vrPicker;    
        public ColorPicker uiPicker;       

        [Header("Paint in 3D 브러시 연결(색 속성 있는 컴포넌트)")]
        [SerializeField] private Component brushComponent;

        [Header("재질 변경 대상(Renderer들)")]
        public Renderer[] materialTargets;

        [Tooltip("URP/Lit = _BaseColor, Standard = _Color, 일부 셰이더 = _TintColor")]
        public string[] colorPropertyPriority = new[] { "_BaseColor", "_Color", "_TintColor" };

        [Header("확정 버튼(색상결정하기) 기능 사용")]
        [Tooltip("true면 '색상결정하기' 버튼을 눌러야만 벽색이 확정됨")]
        public bool applyOnConfirm = true;

        [Header("미리보기 제외 (확정 시엔 적용됨)")]
        public LayerMask previewExcludeLayers;
        public Renderer[] previewExcludeRenderers;

        [Header("UI 패널(색상결정하기 같은 World Canvas)")]
        [Tooltip("첫 페인트 후 띄울 '색상결정하기' 패널 (없으면 비워둬도 됨)")]
        public GameObject decisionPanel;

        [Header("이벤트(필요할 때 UI에 바인딩)")]
        public UnityEvent<string> OnHexChanged;     
        public UnityEvent<Color> OnColorChanged;    

      
        private readonly List<Renderer> _targets = new List<Renderer>();
        private PropertyInfo _brushColorProp;       

        private Color _lastColor = Color.white;     
        private string _lastHex = "#FFFFFF";        
        // 슬롯 계산용 (HexToSlotBridge에서 사용)
        [SerializeField, Range(0f, 1f)]
        private float _lastUVX = 0.5f;

    
        private Color _pendingColor;
        private bool _hasPending;

       

        private void Awake()
        {
            Debug.Log($"[PickerPipeline] Awake on {gameObject.name}");
            CacheBrushProperty();

       
            if (vrPicker != null)
            {
                vrPicker.OnColorPick += HandlePickedColor;
                Debug.Log("[PickerPipeline] VR Picker.OnColorPick 연결 완료");
            }

            
            if (uiPicker != null)
            {
                uiPicker.OnColorPick += HandlePickedColor;
                Debug.Log("[PickerPipeline] UI Picker.OnColorPick 연결 완료");
            }

            CollectTargets();
        }

        private void Start()
        {
           
            if (decisionPanel != null)
            {
                decisionPanel.SetActive(false);
                Debug.Log($"[PickerPipeline] decisionPanel 초기 OFF : {decisionPanel.name}");
            }
        }

        private void OnDestroy()
        {
            if (vrPicker != null)
                vrPicker.OnColorPick -= HandlePickedColor;
            if (uiPicker != null)
                uiPicker.OnColorPick -= HandlePickedColor;
        }

        
        private void HandlePickedColor(Color c)
        {
            Debug.Log($"[PickerPipeline] HandlePickedColor {ColorUtility.ToHtmlStringRGB(c)}");

            
            TrySetBrushColor(c);

           
            _lastColor = c;
            _lastHex = "#" + ColorUtility.ToHtmlStringRGB(c);

            OnColorChanged?.Invoke(c);
            OnHexChanged?.Invoke(_lastHex);

           
            if (applyOnConfirm)
            {
             
                _pendingColor = c;
                _hasPending = true;
                Debug.Log("[PickerPipeline] applyOnConfirm ON → 색은 pending 상태 (벽은 아직 안 바뀜)");
            }
            else
            {
               
                ApplyColorToTargets(c, skipPreviewExcluded: true);
                Debug.Log("[PickerPipeline] 즉시 미리보기 적용");
            }
        }

        
        public void NotifyPaintStroke()
        {
            Debug.Log("[PickerPipeline] NotifyPaintStroke() 호출됨");

            if (decisionPanel == null) return;

            if (!decisionPanel.activeSelf)
            {
                decisionPanel.SetActive(true);
                Debug.Log("[PickerPipeline] decisionPanel.SetActive(true)");
            }
        }

       
        public void ConfirmDecision()
        {
            if (!applyOnConfirm)
            {
                Debug.LogWarning("[PickerPipeline] applyOnConfirm = false 상태라 ConfirmDecision 필요 없음");
                return;
            }

            if (!_hasPending)
            {
                Debug.LogWarning("[PickerPipeline] ConfirmDecision 호출됐지만 pending 색이 없음");
                return;
            }

            
            ApplyColorToTargets(_pendingColor, skipPreviewExcluded: false);

            _hasPending = false;
            if (decisionPanel != null)
                decisionPanel.SetActive(false);

            Debug.Log($"[PickerPipeline] Confirmed -> Applied {_lastHex} to all targets.");
        }

       
        public void ResetForNewSession()
        {
            _hasPending = false;
            if (decisionPanel != null)
                decisionPanel.SetActive(false);
            Debug.Log("[PickerPipeline] ResetForNewSession");
        }

        
        public string GetLastHex() => _lastHex;
        public float GetLastUVX() => _lastUVX;
        public Color GetLastColor() => _lastColor;

        
        private void CacheBrushProperty()
        {
            _brushColorProp = null;
            if (brushComponent == null)
            {
                Debug.LogWarning("[PickerPipeline] brushComponent 비어 있음");
                return;
            }

            _brushColorProp = brushComponent
                .GetType()
                .GetProperty("Color", BindingFlags.Public | BindingFlags.Instance);

            if (_brushColorProp != null)
                Debug.Log("[PickerPipeline] brush Color 프로퍼티 캐시 완료");
            else
                Debug.LogWarning("[PickerPipeline] brushComponent 에 Color 프로퍼티가 없음");
        }

        private void TrySetBrushColor(Color c)
        {
       ...

<...etc...>
