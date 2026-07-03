using UnityEngine;
using UnityEngine.InputSystem;

public class PickToggleBinder : MonoBehaviour
{
    public ColorPicker.ColorPickerVR picker;             
    public InputActionReference toggleAction;              
    public bool startEnabled = false;                      

    void OnEnable()
    {
        if (!picker) return;
        picker.pickerInput = startEnabled;
        toggleAction.action.performed += OnToggle;
        toggleAction.action.Enable();
    }

    void OnDisable()
    {
        toggleAction.action.performed -= OnToggle;
        toggleAction.action.Disable();
    }

    private void OnToggle(InputAction.CallbackContext _)
    {
        if (!picker) return;
        picker.pickerInput = !picker.pickerInput;          
       
    }
}
