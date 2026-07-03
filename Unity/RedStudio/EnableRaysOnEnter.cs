using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnableRaysOnEnter : MonoBehaviour
{
    public XRRayInteractor leftRay, rightRay;
    public XRInteractorLineVisual leftLine, rightLine;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (leftRay) leftRay.enabled = true;
        if (rightRay) rightRay.enabled = true;
        if (leftLine) leftLine.enabled = true;
        if (rightLine) rightLine.enabled = true;
    }
}
