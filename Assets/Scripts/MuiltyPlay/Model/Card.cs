using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Card : IListObject,IComparable
{
    public enum Kind
    {
        Minion,
        Spell,
        Weapon,
        Trap,
        None
    }

    public int Id { get; }
    public int Cost { get; set; }
    public string Description { get;  }
    public string Name { get; }
    public string FunctionDes { get;  }
    public string ImgPath { get;  }
    public bool bNeedTarget = false;
    public Hero.Camp OwnerCamp { get; set; }
    public virtual Kind TypeKind
    {
        get => Kind.None;
    }
    public Action RemoveThisObject { get; set; }
    public Action AddThisObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Card(int id,string name,int cost,string description,string functionDes,string imgPath)
    {
        Id = id;
        Name = name;
        Cost = cost;
        this.Description = description;
        this.FunctionDes = functionDes;
        this.ImgPath = imgPath;
    }

    public Card(Card card)
    {
        Id = card.Id;
        OwnerCamp = card.OwnerCamp;
        Name = card.Name;
        Cost = card.Cost;
        Description = card.Description;
        FunctionDes = card.FunctionDes;
        ImgPath = card.ImgPath;
    }

    public GameObject CreateView()
    {
        var go = new GameObject();
        var text = go.AddComponent<Text>();
        text.text = Name;
        return go;
    }

    public int CompareTo(object obj)
    {
        var card = obj as Card;
        if(card != null)
        {
            return card.Id - Id;
        }
        return -1;
    }
}

