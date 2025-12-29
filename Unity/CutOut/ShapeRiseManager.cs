using System.Collections;
using UnityEngine;

public class ShapeRiseManager : MonoBehaviour
{
    [Header("올라갈 도형들")]
    public Transform[] shapes;

    [Header("기본 상승 설정")]
    public float riseHeight = 0.5f;   
    [Header("트램펄린 모션")]
    public float upExtraHeight = 0.15f; 
    public float downExtraHeight = 0.05f; 

    public float upTime = 0.45f;   
    public float downTime = 0.30f;  

    [Header("상승 후 둥둥 효과 (FloatWobble 블렌드 시간)")]
    public float wobbleBlendTime = 0.4f;

    [Header("안내 UI 시퀀스(선택)")]
    public ShapeInstructionUI instructionUI;

    Vector3[] startLocalPositions;
    FloatWobble[] wobblers;

    public void StartRise()
    {
        StopAllCoroutines();
        StartCoroutine(RiseSequence());
    }

    IEnumerator RiseSequence()
    {
        if (shapes == null || shapes.Length == 0)
            yield break;

        int count = shapes.Length;
        startLocalPositions = new Vector3[count];
        wobblers = new FloatWobble[count];

     
        for (int i = 0; i < count; i++)
        {
            if (shapes[i] == null) continue;

            startLocalPositions[i] = shapes[i].localPosition;

            var wob = shapes[i].GetComponent<FloatWobble>();
            wobblers[i] = wob;

            if (wob != null)
            {
                wob.enabled = false; 
                wob.wobbleWeight = 0f;   
            }
        }

        // 최종 “기준” 위치 = 시작 + riseHeight
        Vector3[] baseEndPos = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            if (shapes[i] == null) continue;
            baseEndPos[i] = startLocalPositions[i] + Vector3.up * riseHeight;
        }

      
        float t = 0f;
        while (t < upTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / upTime);

           
            float e = 1f - Mathf.Pow(1f - k, 3f);

            for (int i = 0; i < count; i++)
            {
                if (shapes[i] == null) continue;

                Vector3 start = startLocalPositions[i];
                Vector3 peak = baseEndPos[i] + Vector3.up * upExtraHeight; 

                shapes[i].localPosition = Vector3.Lerp(start, peak, e);
            }

            yield return null;
        }

    
        for (int i = 0; i < count; i++)
        {
            if (shapes[i] == null) continue;
            Vector3 peak = baseEndPos[i] + Vector3.up * upExtraHeight;
            shapes[i].localPosition = peak;
        }


        t = 0f;
        while (t < downTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / downTime);

            float e = 0.5f - 0.5f * Mathf.Cos(k * Mathf.PI);

            for (int i = 0; i < count; i++)
            {
                if (shapes[i] == null) continue;

                Vector3 peak = baseEndPos[i] + Vector3.up * upExtraHeight;
                Vector3 settlePos = baseEndPos[i] - Vector3.up * downExtraHeight; // 기준보다 조금 아래

                shapes[i].localPosition = Vector3.Lerp(peak, settlePos, e);
            }

            yield return null;
        }

  
        for (int i = 0; i < count; i++)
        {
            if (shapes[i] == null) continue;

            Vector3 settlePos = baseEndPos[i] - Vector3.up * downExtraHeight;
            shapes[i].localPosition = settlePos;
        }

     
        for (int i = 0; i < count; i++)
        {
            var wob = wobblers[i];
            if (wob == null) continue;

          
            wob.enabled = true;        
            wob.ResetPhaseToZero();     
            wob.wobbleWeight = 0f;
        }

        if (wobbleBlendTime > 0f)
        {
            float bt = 0f;
            while (bt < wobbleBlendTime)
            {
                bt += Time.deltaTime;
                float w = Mathf.Clamp01(bt / wobbleBlendTime);

                for (int i = 0; i < count; i++)
                {
                    var wob = wobblers[i];
                    if (wob == null) continue;
                    wob.wobbleWeight = w;
                }

                yield return null;
            }
        }

      
        for (int i = 0; i < count; i++)
        {
            var wob = wobblers[i];
            if (wob == null) continue;
            wob.wobbleWeight = 1f;
        }

       
        if (instructionUI != null)
            instructionUI.PlaySequence();
    }
}
