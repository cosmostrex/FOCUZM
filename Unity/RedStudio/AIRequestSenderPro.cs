using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AIRequestSenderPro : MonoBehaviour
{
    [Header("서버 설정")]
    [Tooltip("예: http://192.168.0.5:8000")]
    public string serverUrl = "http://192.168.0.5:8000";
    public bool logDebug = true;

    [Header("결과 적용 대상 (현재 씬에서는 표시 안함)")]
    public Renderer targetWindowRenderer; 
    public TMP_Text statusText;

    [Header("상태 (응시 결과)")]
    [SerializeField] private WallType _primaryType = WallType.Portrait;
    public WallType PrimaryType => _primaryType;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); 
    }

    
    public void SetPrimaryType(WallType t)
    {
        _primaryType = t;
        if (logDebug) Debug.Log($"[AIRequestSenderPro] PrimaryType <- {t}");
    }

    
    public void SendCompose(string userId, string primaryType, string[] traits)
    {
        serverUrl = serverUrl.Trim();

        if (serverUrl.EndsWith("/"))
            serverUrl = serverUrl.Substring(0, serverUrl.Length - 1);

        if (!serverUrl.StartsWith("http"))
        {
            Debug.LogError("[AIRequestSenderPro] URL이 http로 시작하지 않습니다: " + serverUrl);
            return;
        }

        string finalUrl = $"{serverUrl}/generate-image/";
        Debug.Log("### FINAL URL = " + finalUrl);

        var payload = new ComposePayload
        {
            userId = userId,
            photoId = "multi",
            traits = traits,
            primaryType = primaryType
        };

        string json = JsonUtility.ToJson(payload);

        
        StartCoroutine(PostJson(finalUrl, json, primaryType, traits, json));
    }



    
    private IEnumerator PostJson(string url, string json, string primaryType, string[] traits, string composeJson)
    {
        using var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        if (statusText != null)
        {
            statusText.text = "AI 이미지 생성 중...";
            statusText.color = Color.yellow;
        }

        yield return req.SendWebRequest(); 

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"[AIRequestSenderPro] Error {req.responseCode}: {req.error}\n{req.downloadHandler.text}");
            if (statusText != null)
            {
                statusText.text = $"오류: {req.error}";
                statusText.color = Color.red;
            }
        }
        else
        {
            if (logDebug)
                Debug.Log($"[AIRequestSenderPro] OK {req.responseCode}, length={req.downloadHandler.text.Length}");

            try
            {
                var responseData = JsonUtility.FromJson<ResponseData>(req.downloadHandler.text);

                if (!string.IsNullOrEmpty(responseData.image_data))
                {
                    string traitsCsv = string.Join(", ", traits);
                    long seedVal = 0;
                    long.TryParse(responseData.seed, out seedVal);

   
                    ResultRelay.Instance.SetFromBase64(responseData.image_data, seedVal, primaryType, traitsCsv, composeJson);

                    if (statusText != null)
                    {
                        statusText.text = $"생성 완료! (Seed: {responseData.seed})";
                        statusText.color = Color.green;
                    }

                    Debug.Log($"[AIRequestSenderPro] Image stored in ResultRelay (seed={responseData.seed})");
                }
                else
                {
                    Debug.LogWarning("[AIRequestSenderPro] 응답에 image_data가 없음.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[AIRequestSenderPro] 응답 파싱 오류: {e.Message}");
            }
        }
    }

    [Serializable]
    private class ComposePayload
    {
        public string userId;
        public string photoId;
        public string[] traits;
        public string primaryType;
    }

    [Serializable]
    private class ResponseData
    {
        public string image_data;
        public string seed;
    }
}
