using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterTile : MonoBehaviour
{
    public TextMeshProUGUI letterText;
    private Image background;

    private Color normalColor = new Color(16f / 255f, 101f / 255f, 175f / 255f, 1f); 
    private Color highlightColor = new Color(246f / 255f, 162f / 255f, 0f / 255f, 255f / 255f);

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