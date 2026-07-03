using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectionDetailController : MonoBehaviour
{
    public SelectionAggregator aggregator;

   
    [System.Serializable]
    public class ArtworkPanelEntry
    {
        [Header("어떤 작품인지 (ArtworkKeywords SO)")]
        public ArtworkKeywords keywords;  

        [Header("이 작품 설명용 패널")]
        public GameObject panelRoot;      
    }

    [Header("작품별 패널 리스트")]
    public ArtworkPanelEntry[] artworkPanels;

    
    private GameObject currentPanel;

   
    [Header("락들")]
    public ViewSpotLock startLock;  
    public ViewSpotLock gazeLock;    

    [Header("전환 딜레이")]
    public float moveDelay = 1.0f;

    [Header("XR Rays (UI용)")]
    public XRRayInteractor leftRay, rightRay;
    public XRInteractorLineVisual leftLine, rightLine;

    [Header("컨트롤러 모델 / 도구")]
    public GameObject leftDefaultModel;
    public GameObject rightDefaultModel;
    public GameObject leftPaletteRoot;   
    public GameObject rightBrushModel;  

    [Header("Locomotion")]
    public ContinuousMoveProviderBase moveProvider;   
    public ContinuousTurnProviderBase turnProvider;   

    void Start()
    {
        
        if (artworkPanels != null)
        {
            foreach (var e in artworkPanels)
            {
                if (e != null && e.panelRoot != null)
                    e.panelRoot.SetActive(false);
            }
        }

        currentPanel = null;

        
        ToggleRays(false);
        ToggleTools(false);
    }

    
    public void OpenFor(ArtworkKeywords kw)
    {
        
        if (currentPanel != null)
            currentPanel.SetActive(false);

        currentPanel = null;

        if (kw == null)
        {
            Debug.LogWarning("[SelectionDetailController] kw == null");
        }
        else if (artworkPanels != null)
        {
            
            foreach (var e in artworkPanels)
            {
                if (e == null || e.panelRoot == null || e.keywords == null)
                    continue;

                if (e.keywords == kw)
                {
                    currentPanel = e.panelRoot;
                    break;
                }
            }

            
            if (currentPanel == null && !string.IsNullOrEmpty(kw.artworkId))
            {
                foreach (var e in artworkPanels)
                {
                    if (e == null || e.panelRoot == null || e.keywords == null)
                        continue;

                    if (!string.IsNullOrEmpty(e.keywords.artworkId) &&
                        e.keywords.artworkId == kw.artworkId)
                    {
                        currentPanel = e.panelRoot;
                        break;
                    }
                }
            }
        }

        
        if (currentPanel == null && artworkPanels != null && artworkPanels.Length > 0)
        {
            currentPanel = artworkPanels[0].panelRoot;
            Debug.LogWarning("[SelectionDetailController] 매칭되는 패널이 없어 첫 번째 패널을 사용합니다.");
        }

        if (currentPanel != null)
        {
            currentPanel.SetActive(true);
            Debug.Log($"[SelectionDetailController] OpenFor -> {kw.name} 패널 ON: {currentPanel.name}");
        }
        else
        {
            Debug.LogError("[SelectionDetailController] currentPanel 이 없어서 아무 패널도 못 엽니다.");
        }

        
        ToggleRays(true);
    }

    
    public void OnMore()
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);
    }

    
    public void OnDone()
    {
        
        aggregator?.FinishAndSend();

        
        if (currentPanel != null)
            currentPanel.SetActive(false);

        
        StartCoroutine(ReturnToStartAndEnableTools());
    }

    IEnumerator ReturnToStartAndEnableTools()
    {
        
        ToggleRays(false);
        ToggleDefaults(false);
        ToggleTools(false);

        
        if (gazeLock) gazeLock.Unlock();

        yield return new WaitForSeconds(moveDelay);

        
        if (startLock) startLock.Lock();

        
        if (turnProvider) turnProvider.enabled = false;
        if (moveProvider) moveProvider.enabled = true;

        
        ToggleTools(true);

        
        if (rightRay) rightRay.enabled = true;
        if (rightLine) rightLine.enabled = true;
        
        
    }

    void ToggleRays(bool on)
    {
        if (leftRay) leftRay.enabled = on;
        if (rightRay) rightRay.enabled = on;
        if (leftLine) leftLine.enabled = on;
        if (rightLine) rightLine.enabled = on;
    }

    void ToggleDefaults(bool on)
    {
        if (leftDefaultModel) leftDefaultModel.SetActive(on);
        if (rightDefaultModel) rightDefaultModel.SetActive(on);
    }

    void ToggleTools(bool on)
    {
        if (leftPaletteRoot) leftPaletteRoot.SetActive(on);
        if (rightBrushModel) rightBrushModel.SetActive(on);
    }
}
