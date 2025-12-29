using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneExitController : MonoBehaviour
{
    [Header("�� ���� ���� �� �Ѿ ���� �� �̸�")]
    public string nextSceneName;

    private FadeQuadController fade => FadeQuadController.Instance;

    private void Start()
    {
       
        if (fade != null)
            StartCoroutine(fade.FadeIn());
    }

    public void GoNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("���� �� �̸��� ����ֽ��ϴ�!");
            return;
        }
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
       
        yield return fade.FadeOut();

       
        SceneManager.LoadScene(nextSceneName);
    }
}
