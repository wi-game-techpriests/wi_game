using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainGameController : MonoBehaviour
{
    public GameObject info;
    public GameObject game;
    public GameObject summary;

    public Button retryButton;

    public TMP_Text scoreText; 



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
        int tries = GameManager.Instance.GetCurrentTries();
        if (tries > 0)
        {
            GameManager.Instance.SetCurrentTries();
            info.SetActive(false);
            game.SetActive(true);
            summary.SetActive(false);
        }
    }


    // TODO: visualize MAX score
    public void EndGame()
    {
        int score = GameManager.Instance.GetCurrentScore();
        int tries = GameManager.Instance.GetCurrentTries();

        scoreText.text = score.ToString();
        if (GameManager.Instance.GetSceneResult() < score)
        {
            GameManager.Instance.SetSceneResult(score);
        }

        info.SetActive(false);
        game.SetActive(false);
        summary.SetActive(true);
        if (tries > 0)
        {
            retryButton.gameObject.SetActive(true);
        }
        else
        {
            retryButton.gameObject.SetActive(false);
        }
    }

    public void NextMinigame()
    {
        GameManager.Instance.LoadNextScene();
    }
}
