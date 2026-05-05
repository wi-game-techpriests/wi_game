using UnityEngine;
using UnityEngine.UI;

public class FillInText : MonoBehaviour
{
    public GameObject linePrefab;
    
    public void CreateText(string text, int height, int slotWidth, int padding)
    {



        var parts = text.Split('\n');
        foreach (var part in parts)
        {
            var lineObject = Instantiate(linePrefab,transform);
            var lineScript = lineObject.GetComponent<Line>();
            var lineLayout = lineObject.GetComponent<HorizontalLayoutGroup>();

            lineLayout.spacing = padding;

            lineScript.CreateLine(part,height,slotWidth,padding);
        }

    }
}
