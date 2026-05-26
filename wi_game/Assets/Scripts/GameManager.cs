using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

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

    // Backend communication
    private string backendUrl = "https://wi-game-backend-f608ef6ee0db.herokuapp.com";
    private string sessionToken = "";
    private string sessionCode;
    private string playerNick;    
    // UI References
    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private TextMeshProUGUI errorMessageText;
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
        TMP_InputField codeInput = GameObject.FindWithTag("code").GetComponent<TMP_InputField>();
        TMP_InputField nameInput = GameObject.FindWithTag("name").GetComponent<TMP_InputField>();
        
        sessionCode = codeInput.text;
        playerNick = nameInput.text;
        
        if (string.IsNullOrEmpty(sessionCode) || string.IsNullOrEmpty(playerNick))
        {
            ShowError("Wpisz kod sesji i nick!");
            return;
        }
        
        
        currentIndex = 0;
        currentScore = 0;
        currentTries = 3;

        foreach (string scene in scenes)
        {
            sceneResults[scene] = 0;
        }
        
        StartCoroutine(JoinSession());
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
        if (sceneResults[currentSceneName] < result)
        {
            sceneResults[currentSceneName] = result;
        }
        Debug.Log($"Scene '{currentSceneName}' result saved: {result}");
    }

    public int GetSceneResult()
    {
        return sceneResults.ContainsKey(currentSceneName) ? sceneResults[currentSceneName] : 0;
    }

    public int GetTotalResults()
    {
        int total = 0;
        foreach (var result in sceneResults.Values)
        {
            total += result;
        }
        return total;
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

    // Backend communication
    private IEnumerator JoinSession()
    {
        string url = $"{backendUrl}/sessions/join?code={sessionCode}&nick={playerNick}";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                TokenResponse response = JsonUtility.FromJson<TokenResponse>(responseJson);
                sessionToken = response.token;
                
                StartCoroutine(LoadScene(scenes[currentIndex]));
            }
            else
            {
                long responseCode = request.responseCode;
                string responseText = request.downloadHandler.text;
                string errorMessage = "";
                
                if (responseCode == 403)
                {
                    errorMessage = "Nick '" + playerNick + "' jest zajęty!";
                }
                else if (responseCode == 404)
                {
                    errorMessage = "Kod sesji '" + sessionCode + "' jest niepoprawny!";
                }
                else
                {
                    errorMessage = responseCode + ": " + request.error;
                }
                
                ShowError(errorMessage);
            }
        }

    }

    private void ShowError(string message)
    {
        errorMessageText.text = message;
        popupPrefab.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupPrefab != null)
        {
            popupPrefab.SetActive(false);
        }
    }

    public void GetGameData(string gameType, System.Action<string> onSuccess, System.Action<string> onError = null)
    {
        StartCoroutine(FetchGameData(gameType, onSuccess, onError));
    }

    private IEnumerator FetchGameData(string gameType, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string endpoint = gameType.ToLower() switch
        {
            "wordsearch" => "/game/wordsearch",
            "kahoot" => "/game/kahoot",
            "fill_in" => "/game/fill_in",
            "connections" => "/game/connections",
            _ => "/game/" + gameType
        };

        string url = $"{backendUrl}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + sessionToken);
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonData = request.downloadHandler.text;
                onSuccess?.Invoke(jsonData);
            }
            else
            {
                string errorMsg = $"Błąd {request.responseCode}: {request.error}";
                onError?.Invoke(errorMsg);
            }
        }
    }

    public void GetLeaderboard(string gameType, System.Action<string> onSuccess, System.Action<string> onError = null)
    {
        StartCoroutine(FetchLeaderboard(gameType, onSuccess, onError));
    }

    private IEnumerator FetchLeaderboard(string gameType, System.Action<string> onSuccess, System.Action<string> onError)
    {
        string url = $"{backendUrl}/sessions/leaderboard?gameType={gameType}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", "Bearer " + sessionToken);
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonData = request.downloadHandler.text;
                onSuccess?.Invoke(jsonData);
            }
            else
            {
                string errorMsg = $"Błąd {request.responseCode}: {request.error}";
                onError?.Invoke(errorMsg);
            }
        }
    }

    public void SubmitResults()
    {
        StartCoroutine(SendResults());
    }

    private IEnumerator SendResults()
    {
        var results = new ResultsData
        {
            token = sessionToken,
            connectionsPoints = sceneResults[scenes[0]],
            fillInPoints = sceneResults[scenes[2]],
            wordSearchPoints = sceneResults[scenes[1]],
            kahootPoints = sceneResults[scenes[3]]
         };

        string json = JsonUtility.ToJson(results);
        string url = $"{backendUrl}/game/submit";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", "Bearer " + sessionToken);
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Wyniki wysłane pomyślnie!");
            }
            else
            {
                Debug.LogError($"Błąd wysyłania wyników: {request.responseCode} - {request.error}");
            }
        }
    }
}

[System.Serializable]
public class TokenResponse
{
    public string token;
}

[System.Serializable]
public class ResultsData
{
    public string token;
    public int connectionsPoints;
    public int fillInPoints;
    public int wordSearchPoints;
    public int kahootPoints;
}