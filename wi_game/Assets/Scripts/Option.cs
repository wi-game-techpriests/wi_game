using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Option : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform original;
    public Transform current;
    public TextMeshProUGUI textObject;
    public string text;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(string text)
    {
        this.text = text;
        textObject.text = text;
    }

    public void Snap()
    {
        transform.SetParent(current);
        rectTransform.anchoredPosition = new Vector2(0,0);     
    }

    public void SnapBack()
    {
        ChangeSnap(original);
    }

    public void ChangeSnap(Transform newTransform)
    {
        if (current.TryGetComponent(out Slot oldSlot))
        {
            oldSlot.currentOption = null;
        }
        
        if (newTransform.TryGetComponent(out Slot newSlot))
        {
            if (newSlot.currentOption != null)
            {
                newSlot.currentOption.SnapBack();
            }
            newSlot.currentOption = this;
        }

        current = newTransform;
        Snap();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        Snap();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
