using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    Hero hero;
    public CardListView hand;
    public CardListView battleField;
    public HeroView heroView;
    static bool hasInitAlias = false;
    private void Start()
    {
        if (!hasInitAlias)
        {
            InitPlayerAndReadyToPlay(NetworkPlayer.Instance.hero);
            hasInitAlias = true;
        }
        else
        {
            InitPlayerAndReadyToPlay(GameManager.Inst.GetEnemy(NetworkPlayer.Instance.hero));
            hasInitAlias = false;
        }
    }
    public void RemoveDulpiteHand()
    {

        RemoveSelected();

        List<GameObject> cardReadyToRemove = new List<GameObject>();
        for (int i = 0; i < hand.CardList.Count && cardReadyToRemove.Count != hero.cardNeedToLose; i++)
            if (!cardReadyToRemove.Contains(hand.CardList[i]))
                cardReadyToRemove.Add(hand.CardList[i]);
        hand.RemoveCard(cardReadyToRemove);
    }

    public void RemoveSelected()
    {
        List<GameObject> cardReadyToRemove = new List<GameObject>();
        foreach (var card in hand.CardList)
        {
            if (card.GetComponent<RemoveCardHandler>().bSelected)
            {
                cardReadyToRemove.Add(card);
            }
        }
        hand.RemoveCard(cardReadyToRemove);
        hero.cardSelectedToLose = 0;
        hero.cardNeedToLose -= cardReadyToRemove.Count;
        if (hero.cardNeedToLose > 0)
            TopText.inst.ShowTopText($"需要丢去{hero.cardNeedToLose}张牌");
    }

    public void PrePareLostHand(int x)
    {
        hero.cardNeedToLose = x;
    }

    public void InitPlayerAndReadyToPlay(Hero _hero)
    {
        hero = _hero;
        hand.Camp = hero.m_camp;
        battleField.Camp = hero.m_camp;
        hand.cards = hero.hand;
        battleField.cards = hero.bf;
        heroView.Init(hero);

        hero.onDrawCard += (card) =>
        {
            hand.AddCard(card);
            hand.cards.Add(card);
        };

        hero.onRemoveCard += (cardIndex) =>
        {
            hand.cards.RemoveAt(cardIndex);
            hand.RemoveCard(cardIndex);
        };

        hero.onGameBegin += () =>
        {

            System.Random r = new System.Random();
            const int drawWhenGameBegin = 5;
            if (hero == NetworkPlayer.Instance.hero)
            {
                for (int i = 0; i < drawWhenGameBegin; i++)
                {
                    int cid = r.Next(1, CardDB.inst.GetCardCount());
                    Card card = CardDB.inst.GetCardPrototypeById(cid);
                    if (!NetworkPlayer.Instance.playWithAI)
                        Network.Instance.DrawCardRequest(cid, hero.m_camp);
                    else
                        hero.onDrawCard.Invoke(card);
                }
            }
            else if (NetworkPlayer.Instance.playWithAI)
            {
                for (int i = 0; i < drawWhenGameBegin; i++)
                {
                    int cid = r.Next(1, CardDB.inst.GetCardCount());
                    Card card = CardDB.inst.GetCardPrototypeById(cid);
                    hero.onDrawCard.Invoke(card);
                }
            }
        };

        hero.onTurnBegan += () =>
        {
            System.Random r = new System.Random();
            int cid = r.Next(1, CardDB.inst.GetCardCount());
            Card card = CardDB.inst.GetCardPrototypeById(cid);
            hero.onDrawCard(card);
            Debug.Log($"{hero.m_camp} 回合开始");
        };

        hero.onPlayMinion += (minion, targetIndex) =>
        {
            battleField.DropCardToTargetIndex(targetIndex, minion);
            battleField.cards.Insert(targetIndex, minion);
            Debug.Log($"{hero.m_camp} 部署了 {minion.Name}");
        };


        hero?.onGameBegin();
    }
}
