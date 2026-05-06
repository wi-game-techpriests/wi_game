using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FillInOptions : MonoBehaviour, IDropHandler
{
    public GameObject optionPrefab;
    public GameObject linePrefab;


    public void CreateOptions(string[] options, int perRow, int height, int width)
    {
        if (options.Length == 0) return;
        
        int i = 0;
        while (i + perRow < options.Length)
        {
            var lineObject = Instantiate(linePrefab,transform);
            CreateLine(lineObject,options[i..(i+perRow)],height,width);
            i += perRow;
        }
        
        //Last Row
        {
            var lineObject = Instantiate(linePrefab,transform);
            CreateLine(lineObject,options[i..options.Length],height,width);
        }

        //Reload UI
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            GetComponent<RectTransform>()
        );
    }

    void CreateLine(GameObject line,string[] options, int height, int width)
    {
        foreach (string option in options)
        {
            var optionObject = Instantiate(optionPrefab,line.transform);
            var optionTransform = optionObject.GetComponent<RectTransform>();
            
            //Change Size
            optionTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                height
            );
            optionTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                width
            );


            var optionScript = optionObject.GetComponentInChildren<Option>();
            optionScript.Setup(option);
        }
    }



    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out Option option))
        {
            option.SnapBack();
        }
    }

}
