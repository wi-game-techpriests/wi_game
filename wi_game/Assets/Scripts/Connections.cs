using UnityEngine;
using UnityEngine.UI;

public class Connections : MonoBehaviour
{
    public Image[] images;
    public Sprite unselectedSprite;
    public Sprite selectedSprite;

    private int selectedCount = 0;

    void Start()
    {
        Debug.Log("Connections game started.");
    }

    public void Click(int id)
    {
        if (images[id].sprite == unselectedSprite)
        {
            if (selectedCount >= 4)
            {
                Debug.Log("Cannot select more than 4 connections.");
                return;
            }
            selectedCount++;
            images[id].sprite = selectedSprite;
        }
        else
        {
            selectedCount--;
            images[id].sprite = unselectedSprite;
        }
    }
}
