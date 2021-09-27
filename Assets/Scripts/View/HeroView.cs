using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroView : MonoBehaviour
{
    public Hero hero;
    public Text txtAtk;
    public Text txtHp;
    public AttackHandler attackHandler;
    private void Awake()
    {

    }

    public void Init(Hero hero)
    {
        this.hero = hero;
        attackHandler = GetComponent<AttackHandler>();

        attackHandler.battleAgent = this.hero;


        attackHandler.onAttack += (enemy, go) =>
        {
            hero.Attack(enemy);
        };

        hero.atkChanged += (atk) =>
        {
            txtAtk.gameObject.SetActive(atk > 0);
            txtAtk.text = atk.ToString();
        };

        hero.hpChanged += (hp) =>
        {
            txtHp.text = hp.ToString();
        };

        hero.onDeathComing += () =>
        {
            var enemy = GameManager.Inst.GetEnemy(hero);
            if(enemy.Hp == hero.Hp)
            {
                Debug.Log("平局!");
            }
            else
            {
                if(enemy == NetworkPlayer.Instance.hero)
                {
                    TopText.inst.ShowTopText("胜利!");
                }
                Debug.Log($"{enemy.m_camp} 胜利!");
            }
            Application.Quit();
        };

        hero.Buff(10,30);

    }
}
