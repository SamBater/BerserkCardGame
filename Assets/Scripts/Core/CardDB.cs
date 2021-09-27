using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDB
{
    GameObject cardPrefab;
    GameObject minionPrefab;
    Dictionary<int, Card> cardsDB = new Dictionary<int, Card>();
    public Dictionary<int, Sprite> imgDB = new Dictionary<int, Sprite>();
    static CardDB m_inst;
    Transform canvas = null;

    public static CardDB inst
    {
        get
        {
            if (m_inst == null)
            {
                m_inst = new CardDB();
            }
            return m_inst;
        }
    }

    CardDB()
    {
        LoadFromXlsx();
    }

    public Transform GetCanvas()
    {
        if (canvas == null) canvas = GameObject.Find("Canvas").transform;
        return canvas;
    }
    void LoadFromXlsx()
    {
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        minionPrefab = Resources.Load<GameObject>("Prefabs/Minion");
        TextAsset jsonFile = Resources.Load<TextAsset>("CardDB");
        string json = jsonFile.text;
        JsonReader jsonReader = new JsonReader(json);
        var jsonObjDB = JsonMapper.ToObject(jsonReader);


        for (int i = 0; i < jsonObjDB.Count; i++)
        {
            var item = jsonObjDB[i];
            int id = (int)item["Id"];
            string name = item["Name"].ToString();
            int cost = (int)item["Cost"];
            int atk = (int)item["Atk"];
            int hp = (int)item["Hp"];
            string des = item["Description"].ToString();
            string imgPath = item["ImagePath"].ToString();
            string func = item["Function"].ToString();
            var card = new Card(id, name, cost, des, func, imgPath);
            if (atk >= 0 && hp > 0)
            {
                var minion = new Minion(card, atk, hp);
                cardsDB.Add(id, minion);
            }
            else
                cardsDB.Add(id, card);
        }
    }

    public Card GetCardPrototypeById(int cid)
    {
        if (cardsDB.ContainsKey(cid))
        {
            Card card = null;
            switch (cardsDB[cid])
            {
                case Minion minion:
                    return new Minion(minion);
                case Card c:
                    return c;
            }
            return card;
        }
        else
        {
            throw new Exception("No This Card Id with " + cid);
        }
    }

    public GameObject CreateCardViewInstance(Card card, CardListView cardListView, bool showFront = true)
    {
        var cardGo = GameObject.Instantiate(cardPrefab, cardListView.transform);
        CardView cardView = cardGo.GetComponent<CardView>();
        cardView.ShowBackOnly(!showFront);
        cardView.SetModel(card);
        cardView.OwnerCamp = cardListView.Camp;
        return cardGo;
    }

    public GameObject CreateMinion(Minion minion,CardListView cardListView)
    {
        var cardGo = GameObject.Instantiate(minionPrefab, cardListView.transform);
        CardView cardView = cardGo.GetComponent<CardView>();
        cardView.SetModel(minion);
        cardView.OwnerCamp = cardListView.Camp;
        return cardGo;
    }

    internal int GetCardCount()
    {
        return cardsDB.Count;
    }
}
