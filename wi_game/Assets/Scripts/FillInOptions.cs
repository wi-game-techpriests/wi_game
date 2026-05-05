using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillInOptions : MonoBehaviour
{
    public GameObject optionPrefab;
    public GameObject linePrefab;


    public void CreateOptions(string[] options, int perRow)
    {
        if (options.Length == 0) return;
        
        int i = 0;
        while (i + perRow < options.Length)
        {
            var lineObject = Instantiate(linePrefab,transform);
            CreateLine(lineObject,options[i..(i+perRow)]);
            i += perRow;
        }
        
        {
            var lineObject = Instantiate(linePrefab,transform);
            CreateLine(lineObject,options[i..options.Length]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            GetComponent<RectTransform>()
        );
    }



    public void CreateLine(GameObject line,string[] options)
    {
        foreach (string option in options)
        {
            var optionObject = Instantiate(optionPrefab,line.transform);
            var optionText = optionObject.GetComponentInChildren<TextMeshProUGUI>();
            optionText.text = option;
        }
    }
}
