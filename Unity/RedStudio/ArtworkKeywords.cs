using UnityEngine;

[CreateAssetMenu(fileName = "ArtworkKeywords", menuName = "FOCuzM/Artwork Keywords")]
public class ArtworkKeywords : ScriptableObject
{
    [Header("식별")]
    public string artworkId;      
    public string displayName;    

    [Header("프로프트(한 줄씩)")]
    
    [TextArea(2, 4)] public string portraitPrompt;
    [TextArea(2, 4)] public string stillLifePrompt;
    [TextArea(2, 4)] public string landscapePrompt;
}
