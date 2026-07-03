using UnityEngine;
using ColorPicker;   

public class PaintLockByPicker : MonoBehaviour
{
    [Header("그리기/페인트 관련 컴포넌트 오브젝트")]
    [Tooltip("브러시에 붙어 있는 CW Hit Through 등을 드래그")]
    public Behaviour paintBehaviour;          

    [Header("색 고르는 데 쓰는 컬러피커")]
    [Tooltip("씬에 있는 PickerPipeline 오브젝트를 드래그")]
    public PickerPipeline picker;             

    private bool _hasColor = false;

    void Awake()
    {
        
        if (paintBehaviour != null)
            paintBehaviour.enabled = false;

        
        if (picker != null)
            picker.OnColorChanged.AddListener(OnColorPicked);
    }

    void OnDestroy()
    {
        if (picker != null)
            picker.OnColorChanged.RemoveListener(OnColorPicked);
    }

   
    public void OnColorPicked(Color c)
    {
        _hasColor = true;
        if (paintBehaviour != null)
            paintBehaviour.enabled = true;   
    }


    public void LockPaint()
    {
        _hasColor = false;
        if (paintBehaviour != null)
            paintBehaviour.enabled = false;
    }
}
