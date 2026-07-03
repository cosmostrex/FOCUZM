using UnityEngine;

[CreateAssetMenu(fileName = "ChapelMaterialSwitcher", menuName = "FOCuzM/Chapel/Switcher")]
public class ChapelMaterialSwitcher : ScriptableObject
{
    [Tooltip("슬롯 1~9에 대응하는 머티리얼")]
    public Material[] slotMaterials = new Material[9];

 
    public void ApplySlot(int slot1to9, Renderer r, int submeshIndex = 0)
    {
        if (!r) return;

        int idx = Mathf.Clamp(slot1to9, 1, 9) - 1;

       
        var mat = (slotMaterials != null && idx < slotMaterials.Length)
            ? slotMaterials[idx]
            : null;

        if (!mat)
        {
            Debug.LogWarning($"[ChapelMaterialSwitcher] slot {slot1to9} 에 머티리얼이 없음");
            return;
        }

      
        var mats = r.materials;          
        if (mats == null || mats.Length == 0)
        {
            Debug.LogWarning($"[ChapelMaterialSwitcher] Renderer {r.name} 에 material 이 없음");
            return;
        }

        if (submeshIndex < 0 || submeshIndex >= mats.Length)
            submeshIndex = 0;

        mats[submeshIndex] = mat;
        r.materials = mats;           

       
        Debug.Log($"[ChapelMaterialSwitcher] {r.name} 에 slot {slot1to9} 적용 (submesh {submeshIndex})");
    }
}
