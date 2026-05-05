using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour
{
    public GameObject textPrefab;
    public GameObject slotPrefab;
    public string slotTag;



    public void CreateLine(string rawLine,int height, int slotWidth, int padding)
    {
        string[] parts = rawLine.Split(slotTag);
        for (int i = 0; i < parts.Length; i++)
        {
            //Add text
            if (parts[i].Length > 0)
            {
                var textObject = Instantiate(textPrefab,transform);
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
                textTransform.SetSizeWithCurrentAnchors(
                    RectTransform.Axis.Horizontal,
                    textSize.x
                );
            }
            
            //Add a slot to fill
            if (i < parts.Length - 1)
            {
                var slot = Instantiate(slotPrefab,transform);
                var layout = slot.GetComponent<LayoutElement>();
                //Change size
                layout.preferredHeight = height;
                layout.preferredWidth = slotWidth;
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            GetComponent<RectTransform>()
        );
    }


}
