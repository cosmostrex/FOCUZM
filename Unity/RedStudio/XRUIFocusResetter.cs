using UnityEngine;
using UnityEngine.EventSystems;

public class XRUIFocusResetter : MonoBehaviour
{
    public void ResetFocus()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}
