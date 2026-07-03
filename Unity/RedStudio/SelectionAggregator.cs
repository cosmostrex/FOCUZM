using UnityEngine;
using System.Collections.Generic;

public class SelectionAggregator : MonoBehaviour
{
    public AIRequestSenderPro sender;     
    public GazeWallManager gazeManager;   

    
    private readonly List<ArtworkKeywords> selected = new();
    private readonly HashSet<string> dedup = new();

    public void Begin()
    {
        selected.Clear();
        dedup.Clear();
    }

    
    public void Add(ArtworkKeywords kw)
    {
        if (!kw) return;
        if (selected.Count >= 9) return;
        selected.Add(kw);
        Debug.Log($"[Select +] {kw.name}");
    }

    
    public void FinishAndSend()
    {
        
        gazeManager?.StopAndDecide();

        
        WallType currentWall = gazeManager != null ? gazeManager.GetLongestGaze() : WallType.Portrait;
        string primaryType = currentWall.ToString();

        
        var traits = new List<string>();
        dedup.Clear();

        foreach (var kw in selected)
        {
            string t = PickByPrimary(currentWall, kw);
            t = Normalize(t);
            if (string.IsNullOrEmpty(t)) continue;
            if (dedup.Add(t)) traits.Add(t);
        }

        
        if (sender != null)
        {
            sender.SendCompose(SystemInfo.deviceUniqueIdentifier, primaryType, traits.ToArray());
            Debug.Log($"[Send] primary={primaryType} traits=[{string.Join(", ", traits)}]");
        }
        else
        {
            Debug.LogWarning("AIRequestSenderPro(sender)가 연결되어 있지 않습니다.");
        }

        
        selected.Clear();
        dedup.Clear();
    }

    
    static string PickByPrimary(WallType p, ArtworkKeywords k) =>
        p == WallType.Portrait ? k.portraitPrompt :
        p == WallType.Landscape ? k.landscapePrompt :
        /* StillLife */           k.stillLifePrompt;

    
    static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        while (s.Contains("  ")) s = s.Replace("  ", " ");
        return s;
    }
}
