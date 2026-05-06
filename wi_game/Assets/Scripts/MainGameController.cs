using UnityEngine;

public class MainGameController : MonoBehaviour
{
    public GameObject info;
    public GameObject game;
    public GameObject summary;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        info.SetActive(true);
        game.SetActive(false);
        summary.SetActive(false);
    }


    // TODO: game functionality, like measuring time etc.

    public void StartGame()
    {
        info.SetActive(false);
        game.SetActive(true);
        summary.SetActive(false);
    }

    public void EndGame()
    {
        info.SetActive(false);
        game.SetActive(false);
        summary.SetActive(true);
    }

    public void NextMinigame()
    {
        GameManager.Instance.LoadNextScene();
    }
}
