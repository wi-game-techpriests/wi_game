using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    public string acceptedOption;

    public Option currentOption = null;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out Option option))
        {
            option.ChangeSnap(transform);
        }
    }

    public bool IsCorrect()
    {
        //empty slot
        if (currentOption == null)
        {
            var image = GetComponent<Image>();
            image.color = new Color(0.9f,0,0);
            return false;
        }
        
        //full slot
        bool correct = currentOption.text == acceptedOption;
        var text = GetComponentInChildren<TextMeshProUGUI>();

        if (correct)
        {
            text.color = new Color(0,0.9f,0);
        }
        else
        {
            text.color = new Color(0.9f,0,0);
        }


        //return
        return correct;
    }
}
