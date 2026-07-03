using UnityEngine;

public class ArtworkData : MonoBehaviour
{
    [Header("작품 매칭 정보")]
    [Tooltip("이 작품을 대표하는 키워드를 입력하세요 (예: 해바라기, 별밤, 소녀)")]
    [SerializeField]
    private string artworkKeyword = "DefaultKeyword";

    /// <summary>
    /// 설정된 작품 키워드를 반환합니다.
    /// </summary>
    public string GetKeyword()
    {
        return artworkKeyword;
    }
    
    /// <summary>
    /// 현재 오브젝트 이름과 키워드를 콘솔에 출력합니다.
    /// </summary>
    public void DebugPrintKeyword()
    {
        Debug.Log($"[ArtworkData] 선택된 작품 키워드: {artworkKeyword} (오브젝트: {gameObject.name})");
    }
}
