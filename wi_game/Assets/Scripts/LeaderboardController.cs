using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    public GameObject summary;
    public GameObject leaderboard;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leaderboard.SetActive(false);
        summary.SetActive(true);
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
