using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMSceneAutoPlayer : MonoBehaviour
{
    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public AudioClip bgmClip;
    }

    [Header("���� ������� ���")]
    public SceneBGM[] sceneBGMs;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var entry in sceneBGMs)
        {
            if (entry.sceneName == scene.name)
            {
                BGMFader.Instance.PlayBGM(entry.bgmClip);
                return;
            }
        }
    }
}
