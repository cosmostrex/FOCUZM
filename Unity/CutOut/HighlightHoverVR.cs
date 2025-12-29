using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using HighlightPlus;

[RequireComponent(typeof(HighlightEffect))]
[RequireComponent(typeof(XRBaseInteractable))]  
[DisallowMultipleComponent]
public class HighlightHoverVR : MonoBehaviour
{
    private HighlightEffect effect;
    private XRBaseInteractable interactable;

    [Header("����")]
    public bool highlightOnHover = true;  
    public bool keepOnSelect = true;      

    private bool isSelected = false;

    void Awake()
    {
        effect = GetComponent<HighlightEffect>();
        interactable = GetComponent<XRBaseInteractable>();

        effect.SetHighlighted(false);
    }

    void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
        interactable.selectEntered.AddListener(OnSelectEnter);
        interactable.selectExited.AddListener(OnSelectExit);
    }

    void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
        interactable.selectEntered.RemoveListener(OnSelectEnter);
        interactable.selectExited.RemoveListener(OnSelectExit);
    }

    void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (!highlightOnHover || isSelected) return;
        effect.SetHighlighted(true);
    }

    void OnHoverExit(HoverExitEventArgs args)
    {
        if (isSelected) return;          
        effect.SetHighlighted(false);
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        isSelected = true;
        if (keepOnSelect)
            effect.SetHighlighted(true);
        else
            effect.SetHighlighted(false);
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        isSelected = false;
        effect.SetHighlighted(false);
    }
}
