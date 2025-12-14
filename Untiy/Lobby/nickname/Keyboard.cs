// 2025.10.02
// ShowKeyBoard

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Experimental.UI;

public class ShowKeyBoard : MonoBehaviour
{
    private TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onSelect.AddListener(x => OpenKeyboard());
    }

    // Update is called once per frame
    public void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(inputField.text);
    }
}

// 참고 사이트: https://github.com/Ayfel/MRTK-Keyboard
// https://www.youtube.com/watch?v=irJm8LkkDGw → 13:50/26:22
