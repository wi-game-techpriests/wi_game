using System.Collections.Generic;
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


    private List<Slot> slots;


    void Start()
    {
        Setup();
    }


    public void Setup()
    {
        //Create UI
        slots = textArea.CreateText(text,textHeight,slotWidth,padding);
        optionArea.CreateOptions(options,4,textHeight,slotWidth);

        if (slots.Count > options.Length)
        {
            Debug.LogError("Not enough options");
        }

        //Assign accepted values to slots
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].acceptedOption = options[i];
        }
    }
}
