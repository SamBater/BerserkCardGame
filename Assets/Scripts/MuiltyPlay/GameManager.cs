using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class GameManager
{
    public Hero p1, p2;
    public Hero.Camp TurnOwner;
    static GameManager inst;

    static float secPerTurn = 600;
    static int maxCardInHand = 9;
    public GameManager()
    {
        TurnOwner = Hero.Camp.p1;
    }

    public Hero InitPlayer(Hero.Camp camp)
    {
        var player = new Hero(camp);

        if(camp == Hero.Camp.p1)
        {
            p1 = player;
            p2 = new Hero(Hero.Camp.p2);
        }
        else
        {
            p2 = player;
            p1 = new Hero(Hero.Camp.p1);
        }
        return player;

    }

    public static GameManager Inst
    {
        get
        {
            if(inst == null)
            {
                inst = new GameManager();
            }
            return inst;
        }
    }

    public bool IsTurnOwnner(Hero p)
    {
        return p.m_camp == TurnOwner;
    }

    public void EndTurn()
    {
        var turnOwner = GetPlayer(TurnOwner);
        turnOwner.onTurnEnded?.Invoke();

        turnOwner = turnOwner == p1 ? p2 : p1;
        turnOwner.onTurnBegan?.Invoke();
        TurnOwner = turnOwner.m_camp;
    }

    public Hero GetPlayer(Hero.Camp camp)
    {
        return camp == Hero.Camp.p1 ? p1 : p2;
    }

    public Hero GetEnemy(Hero.Camp yourself)
    {
        return yourself == Hero.Camp.p1 ? p2 : p1;
    }

    public Hero GetEnemy(Hero yourself)
    {
        return yourself == p1 ? p2 : p1;
    }
}