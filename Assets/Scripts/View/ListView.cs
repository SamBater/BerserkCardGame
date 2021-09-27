using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractListViewModel : IListViewModel
{
    public List<IListObject> DataSet { get; set; }

    public event IListViewModel.ItemDelegate onAddItem;
    public event IListViewModel.InsertItemDelegate onInsertItem;
    public event IListViewModel.ItemIndexDelegate onRemoveAtItem;
    public event IListViewModel.ItemDelegate onRemoveItem;

    public AbstractListViewModel()
    {

    }

    public virtual void Add(IListObject item)
    {
        DataSet.Add(item);
        onAddItem.Invoke(item);
        item.RemoveThisObject = () =>
        {
            Remove(item);
        };
    }

    public virtual void Insert(int pos,IListObject item)
    {
        DataSet.Insert(pos, item);
        item.RemoveThisObject = () =>
        {
            Remove(item);
        };
        onInsertItem.Invoke(pos, item);
    }

    public virtual void Remove(IListObject item)
    {
        onRemoveItem.Invoke(item);
        DataSet.Remove(item);
    }

    public virtual void RemoveAt(int pos)
    {
        onRemoveAtItem.Invoke(pos);
        DataSet.RemoveAt(pos);
    }

    public void Sort<T>(Comparison<T> comparison)
    {
        throw new NotImplementedException();
    }
}

public class Hand : AbstractListViewModel
{
    public Hand() 
    {
        DataSet = new List<IListObject>();
    }
    public Hand(List<Card> cards)
    {
        DataSet.AddRange(cards);
    }
}


public interface IListView
{
    void Add(IListObject item);
    void Insert(int pos,IListObject item);
    void Remove(IListObject item);
    void RemoveAt(int pos);
    void Sort<T>(Comparison<T> comparison);
}


public interface IListViewModel
{
    public delegate void ItemDelegate(IListObject item);
    public delegate void ItemIndexDelegate(int pos);
    public delegate void InsertItemDelegate(int pos, IListObject item);
    public event ItemDelegate onAddItem;
    public event InsertItemDelegate onInsertItem;
    public event ItemIndexDelegate onRemoveAtItem;
    public event ItemDelegate onRemoveItem;

    void Add(IListObject item);
    void Insert(int pos, IListObject item);
    void Remove(IListObject item);
    void RemoveAt(int pos);
}

public interface IListObject
{
    public GameObject CreateView();
    public Action RemoveThisObject { get; set; }
    public Action AddThisObject { get; set; }
}

public class ListView : MonoBehaviour
{
    [SerializeField]
    public List<IListObject> DataSet;
    public List<GameObject> Views;

    public void Init(AbstractListViewModel listViewModel)
    {
        Views = new List<GameObject>();
        DataSet = listViewModel.DataSet;
        listViewModel.onAddItem += this.Add;
        listViewModel.onInsertItem += Insert;
        listViewModel.onRemoveItem += Remove;
        listViewModel.onRemoveAtItem += RemoveAt;
    }

    void RemoveAt(int pos)
    {
        if(pos < 0 || pos >= transform.childCount)
        {
            Debug.LogErrorFormat("试图删除索引为[{0}]的物体!", pos);
            return;
        }
        var item = transform.GetChild(pos).gameObject;
        Views.Remove(item);
        DestroyImmediate(item);
    }

    void Insert(int pos,IListObject item)
    {
        if (pos < 0 || pos > transform.childCount)
        {
            Debug.LogErrorFormat("试图在{0}插入超过[{1}]子项长度的Item", name, pos);
            return;
        }

        var go = item.CreateView();
        go.transform.SetParent(transform);
        go.transform.SetSiblingIndex(pos);
        Views.Insert(pos, go);
    }

    void Remove(IListObject item)
    {
        int i = DataSet.IndexOf(item);
        RemoveAt(i);
    }

    void Add(IListObject item)
    {
        Insert(transform.childCount,item);
    }
}
