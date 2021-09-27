using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Minion : Card, IBattleAble
{
    public Minion(Card card):base(card)
    {
        Minion minion = card as Minion;
        if (minion != null)
        {
            m_atk = minion.Atk;
            m_hp = minion.Hp;
        }
    }

    public Minion(Card card,int atk,int hp):base(card)
    {
        m_atk = atk;
        m_hp = hp;
    }

    public Minion(Minion minion):base(minion)
    {
        m_atk = minion.Atk;
        m_hp = minion.Hp;
    }

    int m_hp;
    int m_atk;
    int m_atkChangce = 1;
    public int Hp { get => m_hp; }
    public int Atk { get => m_atk; }

    public int AtkChangce { get => m_atkChangce; set => m_atkChangce = value; }

    public override Kind TypeKind => Kind.Minion;

    public event Action<int> hpChanged;
    public event Action<int> atkChanged;
    public event Action onDeathComing;

    public void Attack(IBattleAble enemy)
    {
        enemy.Hurt(Atk);
        Hurt(enemy.Atk);
    }

    public void Hurt(int damage)
    {
        m_hp = Math.Max(0, Hp - damage);
        hpChanged?.Invoke(m_hp);
        if(Hp <= 0)
        {
            onDeathComing?.Invoke();
        }
    }

    public void Buff(int atk, int hp)
    {
        m_atk += atk;
        m_hp += hp;
        atkChanged?.Invoke(m_atk);
        hpChanged?.Invoke(m_hp);
    }

    public Hero.Camp GetCamp()
    {
        return OwnerCamp;
    }
}
