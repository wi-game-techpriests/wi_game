using UnityEngine;
using UnityEngine.EventSystems;

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
}
