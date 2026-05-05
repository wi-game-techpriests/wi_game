using UnityEngine;

public class FillInPuzzle : MonoBehaviour
{
    public int textHeight;
    public int slotWidth;
    public int padding;

    [TextArea(15,20)]
    public string text;

    public string[] options;

    public FillInText textArea;
    public FillInOptions optionArea;


    void Start()
    {
        Setup();
    }


    public void Setup()
    {
        textArea.CreateText(text,textHeight,slotWidth,padding);
        optionArea.CreateOptions(options,4);
    }
}
