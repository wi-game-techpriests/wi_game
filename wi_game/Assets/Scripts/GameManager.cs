using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string[] scenes;
    public float fadeDuration = 1f;
    private int currentIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        currentIndex = 0;
        StartCoroutine(LoadScene(scenes[currentIndex]));
    }

    public void LoadNextScene()
    {
        currentIndex++;

        if (currentIndex < scenes.Length)
        {
            StartCoroutine(LoadScene(scenes[currentIndex]));
        }
        else
        {
            Debug.Log("No more scenes to load.");
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        AnimationController.Instance.FadeIn();
        yield return null;
        yield return new WaitUntil(() => AnimationController.Instance.FadeInFinished);
        SceneManager.LoadScene(sceneName);
        yield return null;
        AnimationController.Instance.FadeOut();
    }
}