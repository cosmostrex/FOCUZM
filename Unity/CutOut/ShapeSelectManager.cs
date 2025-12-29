using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class ShapeSelectManager : MonoBehaviour
{
    [Header("도형 부모 (shape/대표도형)")]
    [SerializeField] private Transform groupRoot;

    [Header("도형 Tag 이름")]
    [SerializeField] private string shapeTag = "Shape";

    [Header("선택된 도형 목표 위치 (월드 좌표)")]
    public Vector3 focusWorldPosition;  
    public Vector3 focusWorldEuler;  

    [Header("이동 연출")]
    public float moveDuration = 1.2f;
    public float arcHeight = 0.6f;
    public float rotateSpeed = 180f;
    public AnimationCurve moveEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("씬 전환")]
    [Tooltip("다음으로 넘어갈 씬 이름 (예: rosaire)")]
    public string nextSceneName = "rosaire";

    [Tooltip("도형 선택 후 씬 로드까지 대기 시간")]
    public float sceneLoadDelay = 3f;

    [Tooltip("카메라에 붙어 있는 FadeQuadController")]
    public FadeQuadController fadeQuad;  

    private readonly List<Transform> _shapes = new List<Transform>();
    private bool _selectedOnce = false;
    private Transform _selected;

    void OnEnable()
    {
        StartCoroutine(SetupRoutine());
    }

    IEnumerator SetupRoutine()
    {
        while (true)
        {
            BuildShapeList();
            if (_shapes.Count > 0)
            {
                RegisterEvents();
                yield break;
            }
            yield return null;
        }
    }

    void BuildShapeList()
    {
        _shapes.Clear();

        if (groupRoot)
        {
            foreach (var tr in groupRoot.GetComponentsInChildren<Transform>(true))
            {
                if (tr == groupRoot) continue;
                if (tr.CompareTag(shapeTag))
                    _shapes.Add(tr);
            }
        }
        else
        {
            foreach (var go in GameObject.FindGameObjectsWithTag(shapeTag))
                _shapes.Add(go.transform);
        }

        Debug.Log($"[ShapeSelectManager] 찾은 도형 개수: {_shapes.Count}");
    }

    void RegisterEvents()
    {
        foreach (var tr in _shapes)
        {
            if (!tr) continue;

            var interactable = tr.GetComponent<XRBaseInteractable>();
            if (!interactable)
            {
                Debug.LogWarning($"[ShapeSelectManager] {tr.name} 에 XR Interactable 없음");
                continue;
            }

            interactable.selectEntered.RemoveListener(OnShapeSelected);
            interactable.selectEntered.AddListener(OnShapeSelected);
        }
    }

    void OnDisable()
    {
        foreach (var tr in _shapes)
        {
            var inter = tr?.GetComponent<XRBaseInteractable>();
            if (inter) inter.selectEntered.RemoveListener(OnShapeSelected);
        }
    }

    // ★ 실제로 도형을 선택했을 때 호출되는 콜백
    void OnShapeSelected(SelectEnterEventArgs args)
    {
        if (_selectedOnce) return;   
        _selectedOnce = true;

        _selected = args.interactableObject.transform;
        Debug.Log($"[ShapeSelectManager] 선택된 도형: {_selected.name}");

        // ★★★ SceneData 자동 생성 + 값 저장 ★★★
        if (SceneData.Instance == null)
        {
            var go = new GameObject("SceneData");
            go.AddComponent<SceneData>();      
            Debug.Log("[ShapeSelectManager] SceneData가 없어서 새로 생성함.");
        }

        SceneData.Instance.SetSelectedShape(_selected.name);

 

        foreach (var tr in _shapes)
        {
            if (!tr) continue;

          
            var inter = tr.GetComponent<XRBaseInteractable>();
            if (inter) inter.enabled = false;

            var rb = tr.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            if (tr == _selected)
                StartCoroutine(MoveSelectedToFocus(tr));
            else
                tr.gameObject.SetActive(false); // 그냥 사라지게
        }

      
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    IEnumerator MoveSelectedToFocus(Transform shape)
    {
        Vector3 startPos = shape.position;
        Quaternion startRot = shape.rotation;
        Quaternion endRot = Quaternion.Euler(focusWorldEuler);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float k = moveEase.Evaluate(t);

          
            Vector3 mid = (startPos + focusWorldPosition) * 0.5f + Vector3.up * arcHeight;
            Vector3 a = Vector3.Lerp(startPos, mid, k);
            Vector3 b = Vector3.Lerp(mid, focusWorldPosition, k);
            shape.position = Vector3.Lerp(a, b, k);

            shape.rotation = Quaternion.Slerp(startRot, endRot, k);
            shape.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        shape.SetPositionAndRotation(focusWorldPosition, endRot);
        Debug.Log($"[ShapeSelectManager] '{shape.name}' 포커스 위치 도착");
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
     
        if (sceneLoadDelay > 0f)
            yield return new WaitForSeconds(sceneLoadDelay);

       
        if (fadeQuad != null)
        {
            yield return StartCoroutine(fadeQuad.FadeOut());
        }

     
        Debug.Log($"[ShapeSelectManager] '{nextSceneName}' 씬으로 전환");
        SceneManager.LoadScene(nextSceneName);
    }
}
