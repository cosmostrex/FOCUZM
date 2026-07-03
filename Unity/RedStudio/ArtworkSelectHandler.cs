using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRBaseInteractable))]
public class ArtworkSelectHandler : MonoBehaviour
{
    public ArtworkKeywords keywords;
    public SelectionDetailController detailPanel;

    XRBaseInteractable it;

    void Awake()
    {
        it = GetComponent<XRBaseInteractable>();
        it.selectEntered.AddListener(_ => OnSelect());
    }

    void OnSelect()
    {
       
        var ag = FindObjectOfType<SelectionAggregator>();
        ag?.Add(keywords);
        detailPanel?.OpenFor(keywords);
    }
}
