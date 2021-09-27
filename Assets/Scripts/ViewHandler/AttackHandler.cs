using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public event Action<IBattleAble,GameObject> onAttack;
    public IBattleAble battleAgent;

    bool AttackAdition()
    {
        bool isPowerFul = battleAgent.Atk > 0;
        bool isYourSelfTurn = GameManager.Inst.IsTurnOwnner(NetworkPlayer.Instance.hero);
        bool isYourSelfoperator = NetworkPlayer.Instance.camp == battleAgent.GetCamp();
        return isPowerFul && isYourSelfTurn && isYourSelfoperator;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!AttackAdition()) return;
        Arrow.Inst.Show(transform.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Arrow.Inst.Hide();
        if (!AttackAdition()) return;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var i in results)
        {
            var attackHandler = i.gameObject.GetComponent<AttackHandler>();
            if(attackHandler != null)
            {
                var enemyCamp = attackHandler.battleAgent.GetCamp();
                bool isEnemy = battleAgent.GetCamp() != enemyCamp;
                if (!isEnemy) continue;
                onAttack.Invoke(attackHandler.battleAgent,i.gameObject);
                break;
            }
        }
    }
}
