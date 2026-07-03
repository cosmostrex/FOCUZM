using UnityEngine;

public class ChapelSceneApplierSimple : MonoBehaviour
{
    [Header("슬롯 → 머티리얼 매핑 (ScriptableObject)")]
    public ChapelMaterialSwitcher switcher;

    [Header("색을 바꿀 창문 / 스테인드글라스 렌더러들")]
    public Renderer[] targets;

    [Header("바꿀 Submesh Index (보통 0)")]
    public int submeshIndex = 0;

    private bool applied = false;

    private void Start()
    {
        
    }

    public void ApplyFromCarryOver()
    {
        if (applied) return;
        if (!switcher)
        {
            Debug.LogError("[ChapelSceneApplierSimple] switcher null");
            return;
        }

        int slot = Mathf.Clamp(ChapelCarryOver.SelectedSlot, 1, 9);

        foreach (var r in targets)
        {
            if (!r) continue;
            switcher.ApplySlot(slot, r, submeshIndex);
        }

        applied = true;
        Debug.Log($"[ChapelSceneApplierSimple] CLICK 시점에 slot {slot} 적용 완료");
    }
}
