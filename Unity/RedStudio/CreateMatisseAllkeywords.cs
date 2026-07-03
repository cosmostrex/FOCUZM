#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public static class CreateMatisseAllKeywords
{
    [MenuItem("Tools/Keywords/Create Matisse Set (All)")]
    public static void CreateAll()
    {
        string dir = "Assets/Keywords";
        if (!AssetDatabase.IsValidFolder(dir))
            AssetDatabase.CreateFolder("Assets", "Keywords");

       

        
        CreateSO(
            id: "the_goldfish",
            name: "The Goldfish",
            
            portrait: "floral and leaf patterned backdrop",
            stillLife: "goldfish in round fishbowl",
            landscape: "garden and floral",
            path: $"{dir}/AK_TheGoldfish.asset"
        );

        
        CreateSO(
            id: "vase_of_flowers",
            name: "Vase of Flowers",
            
            portrait: "holding a bouquet of mixed blossoms(red, pink, yellow, white)",
            stillLife: "flowers in a rounded vase",
            landscape: "a soft violet sky",
            path: $"{dir}/AK_VaseOfFlowers.asset"
        );

        
        CreateSO(
            id: "still_life_with_oranges",
            name: "Still Life with Oranges",
            
            portrait: "decorative balance",
            stillLife: "pile of oranges with some leaves",
            landscape: "sunlit serenity",
            path: $"{dir}/AK_StillLifeWithOranges.asset"
        );

        

        
        CreateSO(
            id: "the_beach_at_collioure",
            name: "The Beach at Collioure",
            
            portrait: "a background of a Mediterranean seaside bay",
            stillLife: "a ceramic vase in Mediterranean blue tones",
            landscape: "Mediterranean seaside bay",
            path: $"{dir}/AK_TheBeachAtCollioure.asset"
        );

        
        CreateSO(
            id: "view_of_collioure",
            name: "View of Collioure",
           
            portrait: "calm blue-green eyes",
            stillLife: "a vase decorated with a tree motif",
            landscape: "calm blue-green water",
            path: $"{dir}/AK_ViewOfCollioure.asset"
        );

        
        CreateSO(
            id: "open_window_collioure",
            name: "Open Window, Collioure",
            
            portrait: "strong color contrast",
            stillLife: "strong color contrast",
            landscape: "open window with shutters, interior view outward",
            path: $"{dir}/AK_OpenWindowCollioure.asset"
        );

        
        CreateSO(
            id: "woman_by_an_open_window",
            name: "Woman by an Open Window",
            
            portrait: "daylight and intimate calmness",
            stillLife: "a vase decorated with a palm tree",
            landscape: "palm tree beyond balcony",
            path: $"{dir}/AK_WomanByAnOpenWindow.asset"
        );

        

        
        CreateSO(
            id: "the_red_madras_headdress",
            name: "The Red Madras Headdress",
            
            portrait: "woman seated near patterned wall",
            stillLife: "a patterned deep blue tablecloth",
            landscape: "sunlit tropical harmony",
            path: $"{dir}/AK_TheRedMadrasHeaddress.asset"
        );

        
        CreateSO(
            id: "woman_with_a_hat",
            name: "Woman with a Hat",
            
            
            portrait: "patterned dress with bold color blocks",
            stillLife: "a brightly colored tablecloth with striking contrasts",
            landscape: "vivid chromatic landscape",
            path: $"{dir}/AK_WomanWithAHat.asset"
        );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Keywords", "Matisse 키워드 세트(정물/풍경/인물) 9개 생성 완료!", "OK");
    }

    private static void CreateSO(string id, string name,
                                 string portrait, string stillLife, string landscape, string path)
    {
        var so = ScriptableObject.CreateInstance<ArtworkKeywords>();
        so.artworkId = id;
        so.displayName = name;
        
        so.portraitPrompt = portrait;
        so.stillLifePrompt = stillLife;
        so.landscapePrompt = landscape;
        AssetDatabase.CreateAsset(so, path);
    }
}
#endif
