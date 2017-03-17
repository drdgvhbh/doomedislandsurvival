using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler ,IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
    //public static GameObject Tooltip;

    public GameObject SlotParent { get; set; }
    private Vector2 Offset;

    public void OnPointerDown(PointerEventData eventData) {
        Offset = eventData.position - (Vector2)this.transform.position;
        this.transform.position = eventData.position;
        foreach (SlotContainer s in SlotContainer.Pd.SlotContainers) {
            if (s.ItemContainer != null) {
                s.ItemContainer.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }            
    }

    public void OnDrag(PointerEventData eventData) {
        this.transform.position = eventData.position - this.Offset;        
    }

    public void OnEndDrag(PointerEventData eventData) {
        foreach (SlotContainer s in SlotContainer.Pd.SlotContainers) {
            if (s.ItemContainer != null) {
                s.ItemContainer.GetComponent<CanvasGroup>().blocksRaycasts = true;
                s.ItemContainer.transform.position = s.ItemContainer.GetComponent<ItemInput>().SlotParent.transform.position;                
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData) {
       // ItemInput.Tooltip.GetComponent<Tooltip>().Activate(this.Item);
    }

    public void OnPointerExit(PointerEventData eventData) {
       // ItemInput.Tooltip.GetComponent<Tooltip>().DeActivate();
    }

    public void OnPointerUp(PointerEventData eventData) {
            this.OnEndDrag(eventData);
    }
}
