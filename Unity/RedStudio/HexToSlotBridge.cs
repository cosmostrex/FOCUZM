using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ColorPicker; 

public class HexToSlotBridge : MonoBehaviour
{
    [Header("색 정보를 가져올 파이프라인")]
    public PickerPipeline pipeline;   

    [Header("컷아웃 씬 전환 설정")]
    public string cutoutSceneName = "Scenes/Cutout scene";
    public float goDelay = 2f;        

    [Header("슬롯별 대표 Hue (0~1)")]
    [Tooltip("각 슬롯의 '중심 색' Hue 값. 플레이 모드에서 각 슬롯 가운데 찍어서 콘솔에 나온 hue 값을 슬롯별로 넣으면 됨.")]
    [Range(0f, 1f)] public float slot1Hue = 0.03f;   
    [Range(0f, 1f)] public float slot2Hue = 0.10f;   
    [Range(0f, 1f)] public float slot3Hue = 0.17f;   
    [Range(0f, 1f)] public float slot4Hue = 0.25f;  
    [Range(0f, 1f)] public float slot5Hue = 0.33f;   
    [Range(0f, 1f)] public float slot6Hue = 0.42f;  
    [Range(0f, 1f)] public float slot7Hue = 0.52f;   
    [Range(0f, 1f)] public float slot8Hue = 0.70f;   
    [Range(0f, 1f)] public float slot9Hue = 0.85f;   

    [Header("디버그용")]
    [SerializeField] private string _lastHex;
    [SerializeField] private float _lastHue;
    [SerializeField] private int _lastSlot;

   
    public void OnConfirm()
    {
        if (pipeline == null)
        {
            Debug.LogError("[HexBridge] pipeline 이 비어 있음!");
            return;
        }

       
        _lastHex = pipeline.GetLastHex();
        if (!ColorUtility.TryParseHtmlString(_lastHex, out var col))
        {
            Debug.LogError($"[HexBridge] Hex 파싱 실패: {_lastHex}");
            return;
        }

       
        Color.RGBToHSV(col, out float h, out _, out _);
        _lastHue = h;

       
        int slot = HueToNearestSlot(h);
        _lastSlot = slot;

        Debug.Log($"[HexBridge] Confirmed hex = {_lastHex}, hue = {h:0.000} → SLOT = {slot}");

        
        ChapelCarryOver.SetSlot(slot);

       
        StartCoroutine(LoadNextAfterDelay(goDelay));
    }

 
    private int HueToNearestSlot(float h)
    {
        h = Mathf.Repeat(h, 1f);

        float[] centers =
        {
            slot1Hue, slot2Hue, slot3Hue,
            slot4Hue, slot5Hue, slot6Hue,
            slot7Hue, slot8Hue, slot9Hue
        };

        int bestIndex = 0;
        float bestDist = Mathf.Abs(h - centers[0]);

        for (int i = 1; i < centers.Length; i++)
        {
            float d = Mathf.Abs(h - centers[i]);
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }

   
        return bestIndex + 1;
    }

    private IEnumerator LoadNextAfterDelay(float d)
    {
        if (d > 0f)
            yield return new WaitForSeconds(d);

        Debug.Log("[HexBridge] Load scene = " + cutoutSceneName);
        SceneManager.LoadScene(cutoutSceneName);
    }
}
