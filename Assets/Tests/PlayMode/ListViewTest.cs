using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ListViewTest
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestAdd()
    {
        var go = new GameObject();
        var listView = go.AddComponent<ListView>();

        Hand hand = new Hand();
        listView.Init(hand);

        for (int i = 1; i < 10; i++)
        {
            hand.Add(CardDB.inst.GetCardPrototypeById(i));
        }

        Assert.AreEqual(hand.DataSet.Count, listView.transform.childCount);
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestInsert()
    {
        var go = new GameObject();
        var listView = go.AddComponent<ListView>();

        Hand hand = new Hand();
        listView.Init(hand);

        for (int i = 1; i < 5; i++)
        {
            hand.Insert(i - 1,CardDB.inst.GetCardPrototypeById(i));
        }

        for (int i = 1; i < 5; i++)
        {
            hand.Insert(hand.DataSet.Count, CardDB.inst.GetCardPrototypeById(i));
        }

        hand.Insert(6, CardDB.inst.GetCardPrototypeById(4));
        hand.Insert(6, CardDB.inst.GetCardPrototypeById(4));
        hand.Insert(2, CardDB.inst.GetCardPrototypeById(5));

        Assert.AreEqual(hand.DataSet.Count, listView.transform.childCount);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestRemove()
    {
        var go = new GameObject();
        var listView = go.AddComponent<ListView>();

        Hand hand = new Hand();
        listView.Init(hand);

        for (int i = 1; i < 10; i++)
        {
            hand.Add(CardDB.inst.GetCardPrototypeById(i));
        }

        for (int i = 0; i <  8 ;i++)
        {
            hand.Remove(hand.DataSet[0]);
        }

        Debug.Log(string.Format("{0} : {1}", hand.DataSet.Count, listView.transform.childCount));
        Assert.AreEqual(hand.DataSet.Count, listView.transform.childCount);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestModelRemoveSelf()
    {
        var go = new GameObject();
        var listView = go.AddComponent<ListView>();

        Hand hand = new Hand();
        listView.Init(hand);

        for (int i = 1; i < 10; i++)
        {
            hand.Add(CardDB.inst.GetCardPrototypeById(i));
        }

        for (int i = 8; i > 0; i--)
        {
            hand.DataSet[i].RemoveThisObject();
        }

        Assert.AreEqual(hand.DataSet.Count, listView.transform.childCount);

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestRemoveAt()
    {
        var go = new GameObject();
        var listView = go.AddComponent<ListView>();

        Hand hand = new Hand();
        listView.Init(hand);

        for (int i = 1; i < 10; i++)
        {
            hand.Add(CardDB.inst.GetCardPrototypeById(i));
        }

        for (int i = 0; i < 8; i++)
        {
            hand.RemoveAt(0);
        }

        Debug.Log(string.Format("{0} : {1}", hand.DataSet.Count, listView.transform.childCount));
        Assert.AreEqual(hand.DataSet.Count, listView.transform.childCount);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Sort()
    {
        //var go = new GameObject();
        //var listView = go.AddComponent<ListView>();

        //Hand hand = new Hand();
        //listView.Init(hand);

        //for (int i = 1; i < 10; i++)
        //{
        //    hand.Add(CardDB.inst.GetCardPrototypeById(i));
        //}

        //Assert.AreEqual(hand.DataSet.Count, listView.transform.childCount);

        //Comparer<Card> comparer =

        //listView.Sort();

        yield return null;
    }
}
