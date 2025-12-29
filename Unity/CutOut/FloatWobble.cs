using UnityEngine;

[DisallowMultipleComponent]
public class FloatWobble : MonoBehaviour
{
    [Header("��ġ ��鸲(����)")]
    public Vector3 posAmplitude = new Vector3(0f, 0.025f, 0f);  
    public Vector3 posFrequency = new Vector3(0.3f, 0.6f, 0.4f);

    [Header("ȸ�� ��鸲(����, deg)")]
    public Vector3 rotAmplitudeDeg = new Vector3(1.5f, 0.8f, 1.2f);
    public Vector3 rotFrequency = new Vector3(0.2f, 0.35f, 0.28f);

    [Header("���� ������")]
    [Range(0.5f, 1.5f)] public float randomAmpScale = 1f;
    [Range(0.7f, 1.3f)] public float randomFreqScale = 1f;


    [Range(0f, 1f)] public float wobbleWeight = 1f;

    Vector3 _baseLocalPos;
    Quaternion _baseLocalRot;

    float _pPhaseX, _pPhaseY, _pPhaseZ;
    float _rPhaseX, _rPhaseY, _rPhaseZ;

    void OnEnable()
    {
        _baseLocalPos = transform.localPosition;
        _baseLocalRot = transform.localRotation;

       
        RandomizeSeed();
        ResetPhaseToZero();
    }

   
    public void RandomizeSeed()
    {
        randomAmpScale = Random.Range(0.8f, 1.25f);
        randomFreqScale = Random.Range(0.85f, 1.2f);

        _pPhaseX = Random.value * Mathf.PI * 2f;
        _pPhaseY = Random.value * Mathf.PI * 2f;
        _pPhaseZ = Random.value * Mathf.PI * 2f;

        _rPhaseX = Random.value * Mathf.PI * 2f;
        _rPhaseY = Random.value * Mathf.PI * 2f;
        _rPhaseZ = Random.value * Mathf.PI * 2f;
    }

  
    public void ResetPhaseToZero()
    {
        float t = Time.time;

        _pPhaseX = -t * posFrequency.x * randomFreqScale;
        _pPhaseY = -t * posFrequency.y * randomFreqScale;
        _pPhaseZ = -t * posFrequency.z * randomFreqScale;

        _rPhaseX = -t * rotFrequency.x * randomFreqScale;
        _rPhaseY = -t * rotFrequency.y * randomFreqScale;
        _rPhaseZ = -t * rotFrequency.z * randomFreqScale;
    }

    void Update()
    {
        float t = Time.time;

      
        Vector3 wobPos = new Vector3(
            Mathf.Sin(_pPhaseX + t * posFrequency.x * randomFreqScale) * posAmplitude.x,
            Mathf.Sin(_pPhaseY + t * posFrequency.y * randomFreqScale) * posAmplitude.y,
            Mathf.Sin(_pPhaseZ + t * posFrequency.z * randomFreqScale) * posAmplitude.z
        ) * randomAmpScale * wobbleWeight;

     
        Vector3 wobRot = new Vector3(
            Mathf.Sin(_rPhaseX + t * rotFrequency.x * randomFreqScale) * rotAmplitudeDeg.x,
            Mathf.Sin(_rPhaseY + t * rotFrequency.y * randomFreqScale) * rotAmplitudeDeg.y,
            Mathf.Sin(_rPhaseZ + t * rotFrequency.z * randomFreqScale) * rotAmplitudeDeg.z
        ) * randomAmpScale * wobbleWeight;

        transform.localPosition = _baseLocalPos + wobPos;
        transform.localRotation = _baseLocalRot * Quaternion.Euler(wobRot);
    }
}
