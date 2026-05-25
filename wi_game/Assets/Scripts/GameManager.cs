using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string[] scenes;
    [SerializeField] private string endScene;
    public float fadeDuration = 1f;
    private int currentIndex = 0;
    
    // Dictionary to store results for each scene
    private Dictionary<string, int> sceneResults = new Dictionary<string, int>();
    private string currentSceneName = "";


    private int currentScore = 0;
    private int currentTries = 3;

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
        currentScore = 0;
        currentTries = 3;

        foreach (string scene in scenes)
        {
            sceneResults[scene] = 0;
        }
        
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
            StartCoroutine(LoadScene(endScene));
        }

        currentScore = 0;
        currentTries = 3;
    }

    IEnumerator LoadScene(string sceneName)
    {
        AnimationController.Instance.FadeIn();
        yield return null;
        yield return new WaitUntil(() => AnimationController.Instance.FadeInFinished);
        SceneManager.LoadScene(sceneName);
        currentSceneName = sceneName;
        yield return null;
        AnimationController.Instance.FadeOut();
    }

    public void SetSceneResult(int result)
    {
        sceneResults[currentSceneName] = result;
        Debug.Log($"Scene '{currentSceneName}' result saved: {result}");
    }

    public int GetSceneResult()
    {
        return sceneResults.ContainsKey(currentSceneName) ? sceneResults[currentSceneName] : 0;
    }

    public Dictionary<string, int> GetAllResults()
    {
        return new Dictionary<string, int>(sceneResults);
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetCurrentTries()
    {
        return currentTries;
    }

    public void SetCurrentScore(int score)
    {
        currentScore = score;
    }

    public void SetCurrentTries()
    {
        if (currentTries > 0)
        {
            currentTries--;
        }
    }
}