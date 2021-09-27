using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveCardHandler : MonoBehaviour, IPointerDownHandler
{
    public bool bSelected = false;
    public Action onUnSelected;

    public event Func<bool> selecteChecker;
    public event Action onSelected;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (bSelected)
        {
            bSelected = !bSelected;
            transform.position -= Vector3.up * 50;
            onUnSelected?.Invoke();
        }
        else
        {
            if (selecteChecker == null || !selecteChecker.Invoke()) return;
            bSelected = !bSelected;
            transform.position += Vector3.up * 50;
            onSelected?.Invoke();
        }
    }
}
