using Multiplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Network : MonoBehaviour
{

    static Network inst;
    public static Network Instance
    {
        get
        {
            if (inst == null)
            {
                GameObject go = new GameObject();
                inst = go.AddComponent<Network>();
                DontDestroyOnLoad(inst);
            }
            return inst;
        }
    }
    void Start()
    {
        NetworkClient.Register(MessageType.HeartBeat, _Heartbeat);
        NetworkClient.Register(MessageType.Enroll, _Enroll);
        NetworkClient.Register(MessageType.CreatRoom, _CreatRoom);
        NetworkClient.Register(MessageType.EnterRoom, _EnterRoom);
        NetworkClient.Register(MessageType.ExitRoom, _ExitRoom);
        NetworkClient.Register(MessageType.StartGame, _StartGame);
        NetworkClient.Register(MessageType.PlayMinion, _PlayMinion);
        NetworkClient.Register(MessageType.DrawCard, _DrawCard);
        NetworkClient.Register(MessageType.PlayCard, _PlayCard);
        NetworkClient.Register(MessageType.TurnNext, _TurnNext);
        NetworkClient.Register(MessageType.Attack, _Attack);
    }

    private void _Attack(byte[] bytes)
    {
        var receive = NetworkUtils.Deserialize<Attack>(bytes);

        var starter = GameManager.Inst.GetPlayer(receive.StarterCmp);
        var enemy = GameManager.Inst.GetEnemy(starter);

        var attacker = starter.bf[receive.starterIndex] as Minion;
        var defencer = enemy.bf[receive.targetIndex] as Minion;

        attacker.Attack(defencer);
    }

    private void _TurnNext(byte[] bytes)
    {
        GameManager.Inst.EndTurn();
    }

    public void TurnNextRequest()
    {
        if (NetworkPlayer.Instance.playWithAI)
        {
            GameManager.Inst.EndTurn();
            return;
        }

        var request = new TurnNext()
        {
            Roomid = 8848,
        };
        var data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.TurnNext, data);
    }

    private void _PlayCard(byte[] bytes)
    {
        var playCard = NetworkUtils.Deserialize<PlayCard>(bytes);
        var targetHero = GameManager.Inst.GetPlayer(playCard.camp);
        targetHero.onRemoveCard?.Invoke(playCard.handIndex);
    }

    private void _DrawCard(byte[] bytes)
    {
        DrawCard dc = NetworkUtils.Deserialize<DrawCard>(bytes);
        Debug.Log($"{dc.camp} ��ȡ�� {(dc.card != null ? dc.card.Name : "λ�ÿ���")}");
        var player = GameManager.Inst.GetPlayer(dc.camp);
        player.onDrawCard(dc.card);
    }

    /// <summary>
    /// ע��
    /// </summary>
    public void EnrollRequest(string name)
    {
        Enroll request = new Enroll();
        request.Name = name;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.Enroll, data);
    }

    public void DrawCardRequest(int cardId, Hero.Camp camp)
    {
        Card card = CardDB.inst.GetCardPrototypeById(cardId);
        DrawCard dc = new DrawCard();
        dc.card = card;
        dc.RoomId = NetworkPlayer.Instance.RoomId;
        dc.camp = camp;
        byte[] data = NetworkUtils.Serialize(dc);
        NetworkClient.Enqueue(MessageType.DrawCard, data);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void CreatRoomRequest(int roomId)
    {
        CreatRoom request = new CreatRoom();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.CreatRoom, data);
    }

    public void PlayCardRequest(Card card, int index)
    {
        var request = new PlayCard()
        {
            RoomId = 8848,
            camp = NetworkPlayer.Instance.camp == Hero.Camp.p1 ? Hero.Camp.p1 : Hero.Camp.p2,
            card = new Card(card),
            handIndex = index
        };
        var data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.PlayCard, data);
    }

    /// <summary>
    /// ���뷿��
    /// </summary>
    public void EnterRoomRequest(int roomId)
    {
        EnterRoom request = new EnterRoom();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.EnterRoom, data);
    }

    /// <summary>
    /// �˳�����
    /// </summary>
    public void ExitRoomRequest(int roomId)
    {
        ExitRoom request = new ExitRoom();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.ExitRoom, data);
    }

    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    public void StartGameRequest(int roomId)
    {
        StartGame request = new StartGame();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.StartGame, data);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void PlayMinionRequest(Card card, int handIndex, int targetIndex)
    {
        var request = new PlayMinion();
        request.RoomId = 8848;
        request.camp = NetworkPlayer.Instance.camp == Hero.Camp.p1 ? Hero.Camp.p1 : Hero.Camp.p2;
        request.card = new Minion(card);
        request.targetindex = targetIndex;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.PlayMinion, data);
    }

    public void AttackRequest(Hero.Camp camp, int v1, int v2)
    {
        var request = new Attack()
        {
            starterIndex = v1,
            targetIndex = v2,
            StarterCmp = camp,
            Roomid = 8848
        };

        var data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.Attack, data);
    }


    #region ������Ϣ�ص��¼�

    private void _Heartbeat(byte[] data)
    {
        NetworkClient.Received = true;
        Debug.Log("�յ���������Ӧ");
    }

    private void _Enroll(byte[] data)
    {
        Enroll result = NetworkUtils.Deserialize<Enroll>(data);
        if (result.Suc)
        {
            NetworkPlayer.Instance.OnNameChange(result.Name);

            print("ע��ɹ�");
        }
        else
        {
            print("ע��ʧ��");
        }
    }

    private void _CreatRoom(byte[] data)
    {
        CreatRoom result = NetworkUtils.Deserialize<CreatRoom>(data);

        if (result.Suc)
        {
            NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);

            print(string.Format("��������ɹ�, ��ķ������{0}", NetworkPlayer.Instance.RoomId));
        }
        else
        {
            print("��������ʧ��");
        }
    }

    private void _EnterRoom(byte[] data)
    {
        EnterRoom result = NetworkUtils.Deserialize<EnterRoom>(data);

        if (result.result == EnterRoom.Result.Player)
        {
            print("���뷿��ɹ�, ����һ�����");
        }
        else if (result.result == EnterRoom.Result.Observer)
        {
            print("���뷿��ɹ�, ����һ���۲���");
        }
        else
        {
            print("���뷿��ʧ��");
            return;
        }

        //���뷿��
        NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);
    }

    private void _ExitRoom(byte[] data)
    {
        ExitRoom result = NetworkUtils.Deserialize<ExitRoom>(data);

        if (result.Suc)
        {
            //����ű�ΪĬ��
            NetworkPlayer.Instance.OnRoomIdChange(0);
            //���״̬�ı�
            NetworkPlayer.Instance.OnPlayingChange(false);

            print("�˳�����ɹ�");
        }
        else
        {
            print("�˳�����ʧ��");
        }
    }

    private void _StartGame(byte[] data)
    {

        NetworkPlayer.Instance.playWithAI = false;

        StartGame result = NetworkUtils.Deserialize<StartGame>(data);

        if (result.Suc)
        {
            //��ʼ��Ϸ�¼�
            NetworkPlayer.Instance.OnPlayingChange(true);

            //�ǹ۲���
            if (result.Watch)
            {
                //NetworkPlayer.Instance.OnStartGame(Chess.None);
            }
            //�����
            else
            {
                NetworkPlayer.Instance.playWithAI = false;
                NetworkPlayer.Instance.OnStartGame(result.Camp, result.enemyName);
            }
        }
        else
        {
            print("��ʼ��Ϸʧ��");
        }
    }

    private void _PlayMinion(byte[] data)
    {
        var playCard = NetworkUtils.Deserialize<PlayMinion>(data);
        Hero hero = GameManager.Inst.GetPlayer(playCard.camp);
        hero.onPlayMinion?.Invoke(playCard.card as Minion, playCard.targetindex);
    }

    #endregion
}
