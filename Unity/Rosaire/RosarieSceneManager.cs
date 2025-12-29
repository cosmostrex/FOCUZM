using UnityEngine;

public class RosarieSceneManager : MonoBehaviour
{
    [Header("프레임 부모 (예: '프레임' 오브젝트)")]
    public Transform frameParent;

    void Start()
    {
        if (frameParent == null)
        {
            Debug.LogError("[RosarieSceneManager] frameParent가 비어 있음");
            return;
        }

        string selectedShape = null;

        if (SceneData.Instance != null)
        {
            selectedShape = SceneData.Instance.selectedShapeName;
        }

        if (string.IsNullOrEmpty(selectedShape))
        {
            Debug.LogWarning("[RosarieSceneManager] 선택된 도형 정보 없음, 기본값 사용 (henriheart)");
            selectedShape = "henriheart";   // 기본값
        }

        // henristar -> star, henriheart -> heart 이런 식으로 키워드만 뽑기
        string keyword = selectedShape.Replace("henri", "").ToLower();
        Debug.Log($"[RosarieSceneManager] 받은 도형: {selectedShape}, 키워드: {keyword}");

        foreach (Transform child in frameParent)
        {
            child.gameObject.SetActive(false);
        }

        Transform matched = null;

        foreach (Transform child in frameParent)
        {
            string nameLower = child.name.ToLower();

            if (nameLower.Contains(keyword))
            {
                child.gameObject.SetActive(true);
                matched = child;
                Debug.Log($"[RosarieSceneManager] 활성화된 프레임: {child.name}");
            }
        }

        if (matched == null)
        {
            Debug.LogWarning($"[RosarieSceneManager] '{keyword}' 에 해당하는 프레임을 찾지 못했음. 프레임 이름/keyword 확인 필요.");
        }
    }
}
