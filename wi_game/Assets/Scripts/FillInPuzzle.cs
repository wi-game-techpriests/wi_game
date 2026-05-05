using UnityEngine;

public class FillInPuzzle : MonoBehaviour
{
    public int textHeight;
    public int slotWidth;
    public int padding;

    [TextArea(15,20)]
    public string text;

    public FillInText textArea;


    void Start()
    {
        FillText();
    }


    public void FillText()
    {
        textArea.CreateText(text,textHeight,slotWidth,padding);
    }
}
