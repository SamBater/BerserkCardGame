using System;
using System.Collections.Generic;


[Serializable]
public class Hero : IBattleAble
{
    public delegate void BuffMinion(Minion minion,int index,int atk, int hp);
    public enum Camp
    {
        None,p1,p2
    }

    public Camp m_camp;
    public int cardNeedToLose;
    public int cardSelectedToLose;
    public Action<Card> onDrawCard;
    public Action<int> onRemoveCard;
    public Action<Minion,int> onPlayMinion;
    public Action<Minion, int> onRemoveMinion;
    public BuffMinion onMinionBuff;
    public Action onTurnBegan;
    public Action onTurnEnded;
    public Action onGameBegin;
    public List<Card> hand = new List<Card>();
    public List<Card> bf = new List<Card>();
    public Hero(Camp camp)
    {
        m_camp = camp;
        onTurnBegan += () =>
        {
            m_cystal = 10;
        };

        onTurnEnded += () =>
        {
            cardSelectedToLose = 0;
            cardNeedToLose = 0;
        };
    }

    public int m_hp;
    public int m_atk;
    public int m_cystal = 10;
    public int m_atkChangce = 0;

    public int Hp { get => m_hp;  }
    public int Atk { get => m_atk;  }

    public int AtkChangce { get => m_atkChangce; set => m_atkChangce = value; }

    public event Action<int> hpChanged;
    public event Action<int> atkChanged;
    public event Action onDeathComing;

    public void Hurt(int damage)
    {
        m_hp = Math.Max(0, Hp - damage);
        hpChanged?.Invoke(Hp);
        if (Hp <= 0)
            onDeathComing?.Invoke();
    }

    public void Buff(int atk, int hp)
    {
        m_atk += atk;
        m_hp += hp;
        hpChanged?.Invoke(m_hp);
        atkChanged?.Invoke(m_atk);
    }

    public void Attack(IBattleAble enemy)
    {
        enemy.Hurt(Atk);
        Hurt(enemy.Atk);
    }

    public Camp GetCamp()
    {
        return m_camp;
    }
}
