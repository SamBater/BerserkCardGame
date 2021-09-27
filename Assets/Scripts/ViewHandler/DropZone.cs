using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum DropZoneType
    {
        None,
        Hand,
        Field,
    }

    public DropZoneType zoneType;
    public Hero.Camp zoneCamp;
    public void OnDrop(PointerEventData eventData)
    {
        DragAble da = eventData.pointerDrag.GetComponent<DragAble>();
        if (da != null)
        {
            if (da.onDropChecking != null && da.onDropChecking.Invoke(this))
            {
                da.onDrop?.Invoke(this, da.placeHolder.transform.GetSiblingIndex());
                da.backToOrigin = false;
                Destroy(da.placeHolder);
            }
            else
            {
                da.backToOrigin = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        CardView cv = eventData.pointerDrag.GetComponent<CardView>();
        DragAble da = cv.dragAble;
        if (da != null && da.placeHolder != null && cv.Owner.m_camp == zoneCamp)
        {
            da.placeHolder.transform.SetParent(transform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        DragAble da = eventData.pointerDrag.GetComponent<DragAble>();
        if (da != null && da.placeHolder != null)
        {
            da.placeHolder.transform.SetParent(da.parent);
        }
    }
}
