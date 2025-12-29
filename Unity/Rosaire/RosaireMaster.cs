using UnityEngine;

public class RosaireMaster : MonoBehaviour
{
    [Header("씬 시작 시 숨겨둘 것들")]
    public GameObject loraPlane;
    public GameObject cutoutFrameParent;

    [Header("나중에 활성화할 버튼")]
    public GameObject activateButton;

    void Start()
    {
        // 1) 기본 비활성화
        if (loraPlane) loraPlane.SetActive(false);
        if (cutoutFrameParent) cutoutFrameParent.SetActive(false);

        // 2) 컷아웃 프레임 형태 적용
        ShowSelectedFrame();

        // 3) 버튼 처음엔 숨김
        if (activateButton) activateButton.SetActive(false);

        // 4) 나중에 버튼 나타나게 (나레이션 끝 시점 맞추면 됨!)
        Invoke(nameof(ShowActivateButton), 7f);
    }

    void ShowActivateButton()
    {
        if (activateButton) activateButton.SetActive(true);
    }

    public void OnActivateButtonPressed()
    {
        if (loraPlane) loraPlane.SetActive(true);
        if (cutoutFrameParent) cutoutFrameParent.SetActive(true);

        Debug.Log("[RosaireMaster] 최종 이미지 + 프레임 활성화됨!");
    }

    void ShowSelectedFrame()
    {
        string keyword = SceneData.Instance.selectedShapeName
            .Replace("henri", "")
            .ToLower();

        foreach (Transform frame in cutoutFrameParent.transform)
        {
            bool show = frame.name.ToLower().Contains(keyword);
            frame.gameObject.SetActive(show);
        }
    }
}
