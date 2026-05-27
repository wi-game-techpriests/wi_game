using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(RectTransform))]

[RequireComponent(typeof(VerticalLayoutGroup))]
public class FillInText : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject textPrefab;
    public GameObject slotPrefab;
    public string slotTag;

    
    public List<Slot> CreateText(string text, int height, int slotWidth, int padding)
    {
        List<Slot> slots = new();

        var parts = text.Split('\n');
        foreach (var part in parts)
        {
            var lineObject = Instantiate(linePrefab,transform);
            var lineLayout = lineObject.GetComponent<HorizontalLayoutGroup>();

            lineLayout.spacing = padding;

            slots.AddRange(CreateLine(lineObject,part,height,slotWidth,padding));


        }

        //Reload UI (this time fr)
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            GetComponent<RectTransform>()
        );

        return slots;
    }

    public List<Slot> CreateLine(GameObject line, string rawLine,int height, int slotWidth, int padding)
    {
        List<Slot> slots = new();

        string[] parts = rawLine.Split(slotTag);
        for (int i = 0; i < parts.Length; i++)
        {
            //Add text
            if (parts[i].Length > 0)
            {
                var textObject = Instantiate(textPrefab,line.transform);
                var textMesh = textObject.GetComponent<TextMeshProUGUI>();
                var textLayout = textObject.GetComponent<LayoutElement>();
                var textTransform = textObject.GetComponent<RectTransform>();
                //Change height
                textLayout.preferredHeight = height + padding;
                textTransform.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Vertical,
                    height + padding
                );
                //Add text
                textMesh.text = parts[i];
                textMesh.ForceMeshUpdate();
                //Change width
                Vector2 textSize = textMesh.GetRenderedValues(false);
                textLayout.preferredWidth = textSize.x;
                textTransform.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal,
                    textSize.x
                );
            }
            
            //Add a slot to fill
            if (i < parts.Length - 1)
            {
                var slot = Instantiate(slotPrefab,line.transform);
                var layout = slot.GetComponent<LayoutElement>();
                slots.Add(slot.GetComponent<Slot>());
                //Change size
                layout.preferredHeight = height;
                layout.preferredWidth = slotWidth;
            }
        }
        return slots;
    }

    public void ClearText()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
