using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GazeRecorder : MonoBehaviour
{
    [Header("Gaze Recording Settings")]
    [Tooltip("시선이 닿는 오브젝트를 감지하기 위한 최대 거리")]
    public float maxGazeDistance = 10f;
    [Tooltip("유효한 선호 장르로 판단하기 위한 최소 시간 (초)")]
    public float minimumValidTime = 2.0f;

    [Header("Artwork Wall Tags")]
    public string portraitTag = "Wall_Portrait";
    public string landscapeTag = "Wall_Landscape";
    public string stillLifeTag = "Wall_StillLife";

    // 각 태그별 시선 체류 시간을 저장하는 딕셔너리
    private Dictionary<string, float> gazeTimes = new Dictionary<string, float>();

    [Header("Gaze Time Data (Debug)")]
    public float portraitTime = 0f;
    public float landscapeTime = 0f;
    public float stillLifeTime = 0f;

    void Start()
    {
        // 딕셔너리 초기화
        gazeTimes.Add(portraitTag, 0f);
        gazeTimes.Add(landscapeTag, 0f);
        gazeTimes.Add(stillLifeTag, 0f);
    }

    void Update()
    {
        ProcessGazeRecording();
        UpdatePublicTimes();
    }

    /// <summary>
    /// 레이캐스트를 발사하여 사용자가 바라보는 벽면의 태그를 감지하고 시간을 누적합니다.
    /// </summary>
    private void ProcessGazeRecording()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, maxGazeDistance))
        {
            string hitTag = hit.collider.tag;

            if (gazeTimes.ContainsKey(hitTag))
            {
                gazeTimes[hitTag] += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// 인스펙터 창에서 실시간으로 시간을 모니터링할 수 있도록 퍼블릭 변수를 업데이트합니다.
    /// </summary>
    private void UpdatePublicTimes()
    {
        portraitTime = gazeTimes.ContainsKey(portraitTag) ? gazeTimes[portraitTag] : 0f;
        landscapeTime = gazeTimes.ContainsKey(landscapeTag) ? gazeTimes[landscapeTag] : 0f;
        stillLifeTime = gazeTimes.ContainsKey(stillLifeTag) ? gazeTimes[stillLifeTag] : 0f;
    }

    /// <summary>
    /// 설정된 최소 시간(minimumValidTime) 이상 바라본 장르 중, 가장 오래 바라본 선호 장르를 반환합니다.
    /// </summary>
    public string GetPreferredGenre()
    {
        string preferredGenreKey = "None";
        float maxValidTime = 0f;

        foreach (var pair in gazeTimes)
        {
            if (pair.Value >= minimumValidTime)
            {
                if (pair.Value > maxValidTime)
                {
                    maxValidTime = pair.Value;
                    preferredGenreKey = ConvertTagToGenre(pair.Key);
                }
            }
        }
        Debug.Log($"[GazeRecorder] 선호 장르 발견: {preferredGenreKey} (시간: {maxValidTime:F2}초)");
        return preferredGenreKey;
    }

    /// <summary>
    /// 태그명을 알맞은 한글 장르명으로 변환합니다.
    /// </summary>
    private string ConvertTagToGenre(string tag)
    {
        if (tag == portraitTag) return "인물화";
        if (tag == landscapeTag) return "풍경화";
        if (tag == stillLifeTag) return "정물화";
        return "알 수 없음";
    }

    /// <summary>
    /// 누적된 모든 시선 체류 시간을 0으로 리셋합니다.
    /// </summary>
    public void ResetGazeTimes()
    {
        List<string> keys = gazeTimes.Keys.ToList();
        foreach (string key in keys)
        {
             gazeTimes[key] = 0f;
        }
        UpdatePublicTimes();
        Debug.Log("[GazeRecorder] 시선 체류 시간이 초기화되었습니다.");
    }
}
