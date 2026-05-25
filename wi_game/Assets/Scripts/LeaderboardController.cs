using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LeaderboardController : MonoBehaviour
{
    public GameObject summary;
    public GameObject leaderboard;

    public TMP_Text finalScoreText;
    
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private Transform leaderboardContent;
    
    private int totalScore = 0;

    private string currButton = "TOTAL";
    [SerializeField] private Button totalButton;
    [SerializeField] private Button connectionsButton;
    [SerializeField] private Button wordsearchButton;
    [SerializeField] private Button fillinButton;
    [SerializeField] private Button kahootButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalScore = GameManager.Instance.GetTotalResults();
        finalScoreText.text = totalScore.ToString();
        GameManager.Instance.SubmitResults();

        getLeaderboardData("TOTAL");
        leaderboard.SetActive(false);
        summary.SetActive(true);
        totalButton.image.color = new Color32(246, 162, 0, 255);
        connectionsButton.image.color = new Color32(33, 44, 63, 255);
        wordsearchButton.image.color = new Color32(33, 44, 63, 255);
        fillinButton.image.color = new Color32(33, 44, 63, 255);
        kahootButton.image.color = new Color32(33, 44, 63, 255);
    }


    public void Leaderboard()
    {
        leaderboard.SetActive(true);
        summary.SetActive(false);
    }

    public void OpenPage()
    {
        Application.OpenURL("https://www.informatyka.agh.edu.pl/pl/");
    }

    void getLeaderboardData(string query)
    {
        GameManager.Instance.GetLeaderboard(query, 
            (jsonData) => {
                LeaderboardData leaderboardData = JsonUtility.FromJson<LeaderboardData>(jsonData);
                DisplayLeaderboard(leaderboardData);
            },
            (error) => {
                Debug.LogError("Błąd: " + error);
            }
        );
    }

    private void DisplayLeaderboard(LeaderboardData data)
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < data.scores.Length; i++)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            int position = i + 1;

            Image backgroundImage = entry.GetComponent<Image>();
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();
            
            if (texts.Length >= 3)
            {
                texts[0].text = position.ToString();
                
                string playerName = data.scores[i].name;
                if (position-1 == data.playerPosition)
                {
                    playerName += " (ty)";
                }
                texts[1].text = playerName;
                texts[2].text = data.scores[i].score.ToString(); // Wynik
                
                bool isSecondPlace = (position == 2);
                bool isPlayerRow = (position == data.playerPosition);
                
                if (isSecondPlace || isPlayerRow)
                {
                    foreach (var text in texts)
                    {
                        text.color = Color.white;
                    }
                }
                else
                {
                    foreach (var text in texts)
                    {
                        text.color = Color.black;
                    }
                }
            }
            
            if (position == 1)
            {
                if (backgroundImage != null)
                    backgroundImage.color = new Color32(246, 162, 0, 255);
            }
            else if (position == 2)
            {
                if (backgroundImage != null)
                    backgroundImage.color = new Color32(16, 101, 175, 255);
            }
            else
            {
                if (position-1 == data.playerPosition)
                {
                    if (backgroundImage != null)
                        backgroundImage.color = new Color32(33, 44, 63, 255);
                }
                else
                {
                    if (backgroundImage != null)
                        backgroundImage.color = new Color32(133, 185, 229, 255);
                }
                
            }
            
            Debug.Log($"Utworzono element #{position}: {data.scores[i].name} - {data.scores[i].score}");
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(leaderboardContent.GetComponent<RectTransform>()); // Odśwież układ po dodaniu elementów
    }


    public void getTOTAL(){
        if (currButton == "TOTAL") return;
        getLeaderboardData("TOTAL");
        currButton = "TOTAL";
        totalButton.image.color = new Color32(246, 162, 0, 255);
        connectionsButton.image.color = new Color32(33, 44, 63, 255);
        wordsearchButton.image.color = new Color32(33, 44, 63, 255);
        fillinButton.image.color = new Color32(33, 44, 63, 255);
        kahootButton.image.color = new Color32(33, 44, 63, 255);
    }
    public void getCON(){
        if (currButton == "CONNECTIONS") return;
        getLeaderboardData("CONNECTIONS");
        currButton = "CONNECTIONS";
        totalButton.image.color = new Color32(33, 44, 63, 255);
        connectionsButton.image.color = new Color32(246, 162, 0, 255);
        wordsearchButton.image.color = new Color32(33, 44, 63, 255);
        fillinButton.image.color = new Color32(33, 44, 63, 255);
        kahootButton.image.color = new Color32(33, 44, 63, 255);
    }
    public void getWYK(){
        if (currButton == "WORDSEARCH") return;
        getLeaderboardData("WORDSEARCH");
        currButton = "WORDSEARCH";
        totalButton.image.color = new Color32(33, 44, 63, 255);
        connectionsButton.image.color = new Color32(33, 44, 63, 255);
        wordsearchButton.image.color = new Color32(246, 162, 0, 255);
        fillinButton.image.color = new Color32(33, 44, 63, 255);
        kahootButton.image.color = new Color32(33, 44, 63, 255);
    }
    public void getWYP(){
        if (currButton == "FILLIN") return;
        getLeaderboardData("FILLIN");
        currButton = "FILLIN";
        totalButton.image.color = new Color32(33, 44, 63, 255);
        connectionsButton.image.color = new Color32(33, 44, 63, 255);
        wordsearchButton.image.color = new Color32(33, 44, 63, 255);
        fillinButton.image.color = new Color32(246, 162, 0, 255);
        kahootButton.image.color = new Color32(33, 44, 63, 255);
    }
    public void getKAH(){
        if (currButton == "KAHOOT") return;
        getLeaderboardData("KAHOOT");
        currButton = "KAHOOT";
        totalButton.image.color = new Color32(33, 44, 63, 255);
        connectionsButton.image.color = new Color32(33, 44, 63, 255);
        wordsearchButton.image.color = new Color32(33, 44, 63, 255);
        fillinButton.image.color = new Color32(33, 44, 63, 255);
        kahootButton.image.color = new Color32(246, 162, 0, 255);
    }
}





// Struktury do parsowania JSON leaderboard'u
[System.Serializable]
public class ScoreEntry
{
    public string name;
    public int score;
}

[System.Serializable]
public class LeaderboardData
{
    public int playerId;
    public string playerName;
    public int playerPosition;
    public ScoreEntry[] scores;
}
