using System.Collections.Generic;
using UnityEngine;

public class CardListView : MonoBehaviour
{
    public List<Card> cards = new List<Card>();
    DropZone dropZone;
    public List<GameObject> CardList
    {
        get
        {
            List<GameObject> cardList = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                cardList.Add(transform.GetChild(i).gameObject);
            }
            return cardList;
        }
    }

    public DropZone.DropZoneType DropZoneType
    {
        get => dropZone.zoneType;
        set => dropZone.zoneType = value;
    }

    public Hero.Camp Camp
    {
        get => dropZone.zoneCamp;
        set => dropZone.zoneCamp = value;
    }

    private void Awake()
    {
        dropZone = GetComponent<DropZone>();
    }

    public void DropCardToTargetIndex(int index, Card card)
    {
        var go = CardDB.inst.CreateMinion(card as Minion, this);
        go.transform.SetSiblingIndex(index);
        var cv = go.GetComponent<CardView>();
        if (DropZoneType == DropZone.DropZoneType.Field)
        {
            cv.SubmitAttackEvent();
        }
        if (DropZoneType == DropZone.DropZoneType.Hand)
        {
            cv.SubmitEvent();
        }
    }

    public void AddCard(Card card)
    {
        if (DropZoneType == DropZone.DropZoneType.Field)
        {
            var cardGo = CardDB.inst.CreateMinion(card as Minion, this);
            var cv = cardGo.GetComponent<CardView>();
            cv.SubmitAttackEvent();
        }
        else if (DropZoneType == DropZone.DropZoneType.Hand)
        {
            var cardGo = CardDB.inst.CreateCardViewInstance(card, this);
            var cv = cardGo.GetComponent<CardView>();
            cv.ShowBackOnly(Camp != NetworkPlayer.Instance.camp);
            cv.SubmitEvent();
            cv.imgCost.transform.localScale = Vector3.one * 1.5f;
        }

    }

    public void RemoveCard(List<GameObject> cardList)
    {
        foreach (var card in cardList)
            RemoveCard(card);
    }

    public void RemoveCard(GameObject card)
    {
        Destroy(card);
    }

    public void RemoveCard(int cardIndex)
    {
        Destroy(transform.GetChild(cardIndex).gameObject);
    }

}
