using System;
using System.Collections;
using System.Collections.Generic;

public interface IBattleAble
{
    event Action<int> hpChanged;
    event Action<int> atkChanged;
    event Action onDeathComing;
    int Hp { get; }
    int Atk { get; }
    int AtkChangce { get; }
    void Attack(IBattleAble enemy);
    void Hurt(int damage);
    void Buff(int atk, int hp);
    Hero.Camp GetCamp();
}
