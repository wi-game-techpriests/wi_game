using UnityEngine;
using UnityEngine.UI;

public class Lifes : MonoBehaviour
{
    public Image[] lifeImages;
    public Sprite liveSprite;
    public Sprite deadSprite;

    private int currentTry = 3;

    public void Check()
    {
        // Fail
        if (currentTry > 0)
        {
            lifeImages[currentTry].sprite = deadSprite;
            Debug.Log("Wrong answer. Remaining tries: " + currentTry);
            currentTry--;
        }
        else
        {
            lifeImages[0].sprite = deadSprite;
            Debug.Log("Game Over!");
            MainGameController mainGameController = FindAnyObjectByType<MainGameController>();
            mainGameController.EndGame();
        }
    }
}
