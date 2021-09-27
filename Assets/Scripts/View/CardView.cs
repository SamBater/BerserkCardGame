using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region member_vars
    public Card card;
    public Image img;
    public Image imgName;
    public Image imgDes;
    public Image imgBack;
    public Image imgCost;
    public Image imgAtk;
    public Image imgDef;
    public Text txtName;
    public Text txtDes;
    public Text txtAtk;
    public Text txtHp;
    public Text txtCost;
    public AudioSource audioSource;
    public DragAble dragAble;
    public RemoveCardHandler rh;
    public AttackHandler atkHandler;

    public static GameObject LastDrop { get; set; }
    public DropZone.DropZoneType Zone
    {
        get => GetComponentInParent<DropZone>().zoneType;
    }

    [SerializeField]
    public Hero.Camp OwnerCamp
    {
        get => card.OwnerCamp;
        set => card.OwnerCamp = value;
    }

    public Hero Owner
    {
        get => GameManager.Inst.GetPlayer(OwnerCamp);
    }

    #endregion

    #region FunctionForEventSubmit
    public void SubmitEvent()
    {
        SubmitDragAble();
        SubmitRemoveHandler();

    }

    private void SubmitDragAble()
    {
        dragAble = gameObject.AddComponent<DragAble>();
        if (dragAble)
        {
            dragAble.onBeginDropChecking += () =>
            {
                if (Zone == DropZone.DropZoneType.Field)
                {
                    return false;
                }

                if (NetworkPlayer.Instance.hero != Owner)
                {
                    Debug.Log("还想弄爷的牌???");
                    return false;
                }

                if (!GameManager.Inst.IsTurnOwnner(Owner))
                {
                    TopText.inst.ShowTopText("现在不是你的回合!");
                    return false;
                }

                return true;
            };

            dragAble.onDropChecking += (DropZone dropZone) =>
            {
                if (Owner.m_cystal < card.Cost)
                {
                    TopText.inst.ShowTopText("水晶不足!");
                    return false;
                }
                return dropZone.zoneCamp == OwnerCamp && dropZone.zoneType == DropZone.DropZoneType.Field;
            };

            dragAble.onDrop += (dropZone, targetIndex) =>
            {
                if (!NetworkPlayer.Instance.playWithAI)
                {
                    Network.Instance.PlayCardRequest(card, dragAble.index);
                }
                //owner.m_cystal -= m_card.Cost;
                switch (card)
                {
                    case Minion minion:
                        if (NetworkPlayer.Instance.playWithAI)
                        {
                            Owner.onPlayMinion?.Invoke(minion, targetIndex);
                        }
                        else
                        {
                            Network.Instance.PlayMinionRequest(card, dragAble.index, targetIndex);
                        }
                        break;
                    default:
                        break;
                }
                Destroy(gameObject);
            };
        }
    }

    private void SubmitRemoveHandler()
    {
        rh = gameObject.AddComponent<RemoveCardHandler>();
        if (rh)
        {
            rh.selecteChecker += () =>
            {
                return Owner.cardSelectedToLose < Owner.cardNeedToLose;
            };

            rh.onSelected += () =>
            {
                Owner.cardSelectedToLose++;
            };

            rh.onUnSelected += () =>
            {
                Owner.cardSelectedToLose--;
            };
        }
    }
    public void SubmitAttackEvent()
    {
        if (card is Minion)
        {
            atkHandler = gameObject.AddComponent<AttackHandler>();
            atkHandler.battleAgent = card as Minion;

            atkHandler.onAttack += (enemy,go) =>
            {
                var minion = card as Minion;
                if (NetworkPlayer.Instance.playWithAI)
                    minion.Attack(enemy);
                else
                    Network.Instance.AttackRequest(OwnerCamp, transform.GetSiblingIndex(), go.transform.GetSiblingIndex());

                audioSource.clip = Resources.Load<AudioClip>("Sound/Attack");
                audioSource.Play();

                Debug.Log($"{OwnerCamp} {minion.Name} Attack {enemy.GetCamp()} {enemy} !");
            };

            (card as Minion).onDeathComing += () =>
            {
                audioSource.clip = Resources.Load<AudioClip>("Sound/Death");
                audioSource.Play();
                Debug.Log($"{OwnerCamp} {card.Name} is dead.");
                GameManager.Inst.GetPlayer(OwnerCamp).bf.Remove(card);
                Destroy(gameObject,0.5f);
            };
        }
        Destroy(rh);
        StartCoroutine("DeleteDropAble");
        ChangeOutlookForBF();
    }
    #endregion
    IEnumerator DeleteDropAble()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(dragAble);
    }

    public void ShowBackOnly(bool val)
    {
        imgBack.gameObject.SetActive(val);
        imgCost?.gameObject.SetActive(!val);
        imgAtk?.gameObject.SetActive(!val);
        imgDef?.gameObject.SetActive(!val);
    }

    public void ChangeOutlookForBF()
    {
        if (card.TypeKind != Card.Kind.Minion) return;
        ShowBackOnly(false);
        txtAtk.gameObject.transform.localScale = Vector3.one * 2f;
        txtHp.gameObject.transform.localScale = Vector3.one * 2f;
        imgName.gameObject.SetActive(false);
        imgDes.gameObject.SetActive(false);
        imgCost.gameObject.SetActive(false);
        imgAtk.color = imgDef.color = new Color(255, 255, 255, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null) return;
        if (Owner.m_camp != NetworkPlayer.Instance.camp && Zone == DropZone.DropZoneType.Hand) return;
        {
            var um = CardDB.inst.GetCanvas().GetComponent<UIEventManager>();
            um.ShowBig(true, card);
        }

    }

    public void SetModel(Card card)
    {
        this.card = card;
        name = this.card.Name;
        txtName.text = this.card.Name;
        txtDes.text = this.card.Description;
        txtCost.text = this.card.Cost.ToString();


        Dictionary<int, Sprite> imgDB = CardDB.inst.imgDB;
        if (!imgDB.ContainsKey(this.card.Id))
            imgDB.Add(this.card.Id, Resources.Load<Sprite>(card.ImgPath));
        img.sprite = imgDB[this.card.Id];

        bool isMinion = card.TypeKind == Card.Kind.Minion;
        imgAtk.gameObject.SetActive(isMinion);
        imgDef.gameObject.SetActive(isMinion);

        if (card.TypeKind == Card.Kind.Minion)
        {
            Minion minion = card as Minion;
            txtAtk.text = minion.Atk.ToString();
            txtHp.text = minion.Hp.ToString();
            minion.atkChanged += (val) =>
            {
                if(txtAtk != null)
                txtAtk.text = val.ToString();
            };
            minion.hpChanged += (val) =>
            {
                if(txtHp != null)
                txtHp.text = val.ToString();
            };
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var um = CardDB.inst.GetCanvas().GetComponent<UIEventManager>();
        um.ShowBig(false);
    }
}
