using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardController : MonoBehaviour
{
    public GameObject summary;
    public GameObject leaderboard;

    public TMP_Text finalScoreText;
    
    private int totalScore = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalScore = GameManager.Instance.GetTotalResults();
        finalScoreText.text = totalScore.ToString();

        leaderboard.SetActive(false);
        summary.SetActive(true);
        // GameManager.Instance.GetLeaderboard("CONNECTIONS", 
        //     (jsonData) => {
        //         Debug.Log("Leaderboard CONNECTIONS: " + jsonData);
        //         // Tu parsuj JSON i wyświetl wyniki
        //     },
        //     (error) => {
        //         Debug.LogError("Błąd: " + error);
        //     }
        // );
        
    }


    // TODO: leaderboard functionality

    
    public void Leaderboard()
    {
        leaderboard.SetActive(true);
        summary.SetActive(false);
    }

    public void OpenPage()
    {
        Application.OpenURL("https://www.informatyka.agh.edu.pl/pl/");
    }
}
