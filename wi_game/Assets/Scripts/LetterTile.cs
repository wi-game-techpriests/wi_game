using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterTile : MonoBehaviour
{
    public TextMeshProUGUI letterText;
    private Image background;

    private Color normalColor = new Color(1f, 1f, 1f, 100f/255f); 
    private Color highlightColor = new Color(1f, 0.92f, 0.016f, 1f); 

    void Awake()
    {
        background = GetComponent<Image>();
        if (background != null) background.color = normalColor;
    }

    public void SetHighlight(bool state)    
    {
        if (background != null)
        {
            background.color = state ? highlightColor : normalColor;
        }
    }

    public void SetLetter(string letter)
    {
        if (letterText != null)
            letterText.text = letter;
    }

    public string GetLetter()
    {
        return letterText != null ? letterText.text : "";
    }
}