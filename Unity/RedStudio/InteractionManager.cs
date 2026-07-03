using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class InteractionManager : MonoBehaviour
{
    [Header("Dependencies")]
    public GazeRecorder gazeRecorder;

    [Header("Data Saving Settings")]
    [Tooltip("데이터를 저장할 파일 경로 (예: D:/VR_Gaze_Data.csv)")]
    public string customSavePath = "D:/VR_Gaze_Data.csv";

    [Header("Interaction Settings")]
    [Tooltip("상호작용이 가능한 최대 레이캐스트 거리")]
    public float maxInteractionDistance = 10f;

    private string saveFilePath;

    void Start()
    {
        saveFilePath = customSavePath;
        Debug.Log($"Data will be saved to: {saveFilePath}");

        // 파일이 없으면 CSV 헤더(타이틀) 작성
        if (!File.Exists(saveFilePath))
        {
            File.WriteAllText(saveFilePath, "SessionID,FirstKeyword_Genre,SecondKeyword_Artwork,Time_Portrait,Time_Landscape,Time_StillLife,Timestamp\n");
        }

        if (gazeRecorder == null) 
            Debug.LogError("GazeRecorder is not connected on InteractionManager!");
    }

    void Update()
    {
        // 스페이스바를 누르면 현재 바라보는 작품을 선택 및 기록
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ProcessArtworkSelection();
        }
    }

    private void ProcessArtworkSelection()
    {
        RaycastHit hit;
        // 메인 카메라를 기준으로 정면으로 레이를 발사
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxInteractionDistance))
        {
            ArtworkData artworkData = hit.collider.GetComponent<ArtworkData>();

            if (artworkData != null)
            {
                // 데이터 수집
                string firstKeyword = gazeRecorder.GetPreferredGenre();
                string secondKeyword = artworkData.GetKeyword();

                float timeP = gazeRecorder.portraitTime;
                float timeL = gazeRecorder.landscapeTime;
                float timeS = gazeRecorder.stillLifeTime;

                if (firstKeyword == "None")
                {
                    Debug.LogWarning("⚠️ 유효한 장르 정보(2초 이상 체류)를 찾지 못했습니다. 작품 키워드만 기록됩니다.");
                }

                // CSV 파일에 저장
                SaveDataToCSV(firstKeyword, secondKeyword, timeP, timeL, timeS);

                Debug.Log($"📊 DATA COLLECTED AND SAVED: 장르({firstKeyword}), 작품({secondKeyword})");
            }
            else
            {
                Debug.Log("🔍 레이캐스트에 감지된 오브젝트가 ArtworkData를 가지고 있지 않습니다.");
            }
        }
        else
        {
            Debug.Log("❌ 아무것도 선택되지 않았습니다.");
        }
    }

    private void SaveDataToCSV(string genre, string artwork, float timeP, float timeL, float timeS)
    {
        string sessionID = Guid.NewGuid().ToString();
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string newLine = $"{sessionID},{genre},{artwork},{timeP:F2},{timeL:F2},{timeS:F2},{timestamp}\n";

        try
        {
            File.AppendAllText(saveFilePath, newLine);
            Debug.Log($"💾 Data Saved to CSV at {saveFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving data to {saveFilePath}. Check if D: drive exists and folder permissions are correct. Error: {e.Message}");
        }
    }
}
