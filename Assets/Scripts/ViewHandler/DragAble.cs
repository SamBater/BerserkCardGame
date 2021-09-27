using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragAble : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parent;
    public GameObject placeHolder;
    CanvasGroup canvasGroup;
    public int index = 0;
    public bool backToOrigin = true;
    public Func<bool> onBeginDropChecking;
    public Func<DropZone, bool> onDropChecking;
    public Func<bool> onChangeIndexChecking;
    public Action<DropZone, int> onDrop;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (onBeginDropChecking != null && !onBeginDropChecking.Invoke())
        {
            eventData.pointerDrag = null;
            return;
        }
        index = transform.GetSiblingIndex();
        parent = transform.parent;
        Transform target = CardDB.inst.GetCanvas();
        transform.SetParent(target);
        canvasGroup.blocksRaycasts = false;
        //TODO Change This To Univsesal. which mean,Invisible is a abstrcut function,and alos cardview.
        placeHolder = GameObject.Instantiate(gameObject, target);
        placeHolder.GetComponent<Image>().color = new Color(0, 0, 0, 0);


        placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!placeHolder) return;
        transform.position = eventData.position;

        //var cardview = placeHolder.GetComponent<CardView>();
        //var pr = cardview.GetComponentInParent<DropZone>();
        //if (pr)
        //{
        //    if(pr.zoneType == DropZone.DropZoneType.Hand || pr.zoneCamp != cardview.Camp)
        //    {
        //        placeHolder.transform.SetSiblingIndex(index);
        //        return;
        //    }
        //}
        //placeHolder.transform.SetSiblingIndex(index);
        //条件

        for (int i = 0; i < placeHolder.transform.parent.childCount; i++)
        {
            float x = placeHolder.transform.parent.GetChild(i).position.x;
            //找到第一个拖拽物右侧第一卡.
            if (x > transform.position.x)
            {

                if (placeHolder.transform.GetSiblingIndex() < i)
                    i--;
                placeHolder.transform.SetSiblingIndex(i);
                return;
            }
            if (i + 1 == placeHolder.transform.parent.childCount)
                placeHolder.transform.SetSiblingIndex(i);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!placeHolder) return;
        canvasGroup.blocksRaycasts = true;

        if (backToOrigin)
        {
            transform.parent = parent;
            transform.SetSiblingIndex(index);
        }
        Destroy(placeHolder);
    }
}
