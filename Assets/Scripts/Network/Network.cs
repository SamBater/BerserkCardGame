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
        Debug.Log($"{dc.camp} 抽取了 {(dc.card != null ? dc.card.Name : "位置卡牌")}");
        var player = GameManager.Inst.GetPlayer(dc.camp);
        player.onDrawCard(dc.card);
    }

    /// <summary>
    /// 注册
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
    /// 创建房间
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
    /// 加入房间
    /// </summary>
    public void EnterRoomRequest(int roomId)
    {
        EnterRoom request = new EnterRoom();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.EnterRoom, data);
    }

    /// <summary>
    /// 退出房间
    /// </summary>
    public void ExitRoomRequest(int roomId)
    {
        ExitRoom request = new ExitRoom();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.ExitRoom, data);
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    public void StartGameRequest(int roomId)
    {
        StartGame request = new StartGame();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.StartGame, data);
    }

    /// <summary>
    /// 下棋请求
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


    #region 发送消息回调事件

    private void _Heartbeat(byte[] data)
    {
        NetworkClient.Received = true;
        Debug.Log("收到心跳包回应");
    }

    private void _Enroll(byte[] data)
    {
        Enroll result = NetworkUtils.Deserialize<Enroll>(data);
        if (result.Suc)
        {
            NetworkPlayer.Instance.OnNameChange(result.Name);

            print("注册成功");
        }
        else
        {
            print("注册失败");
        }
    }

    private void _CreatRoom(byte[] data)
    {
        CreatRoom result = NetworkUtils.Deserialize<CreatRoom>(data);

        if (result.Suc)
        {
            NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);

            print(string.Format("创建房间成功, 你的房间号是{0}", NetworkPlayer.Instance.RoomId));
        }
        else
        {
            print("创建房间失败");
        }
    }

    private void _EnterRoom(byte[] data)
    {
        EnterRoom result = NetworkUtils.Deserialize<EnterRoom>(data);

        if (result.result == EnterRoom.Result.Player)
        {
            print("加入房间成功, 你是一名玩家");
        }
        else if (result.result == EnterRoom.Result.Observer)
        {
            print("加入房间成功, 你是一名观察者");
        }
        else
        {
            print("加入房间失败");
            return;
        }

        //进入房间
        NetworkPlayer.Instance.OnRoomIdChange(result.RoomId);
    }

    private void _ExitRoom(byte[] data)
    {
        ExitRoom result = NetworkUtils.Deserialize<ExitRoom>(data);

        if (result.Suc)
        {
            //房间号变为默认
            NetworkPlayer.Instance.OnRoomIdChange(0);
            //玩家状态改变
            NetworkPlayer.Instance.OnPlayingChange(false);

            print("退出房间成功");
        }
        else
        {
            print("退出房间失败");
        }
    }

    private void _StartGame(byte[] data)
    {

        NetworkPlayer.Instance.playWithAI = false;

        StartGame result = NetworkUtils.Deserialize<StartGame>(data);

        if (result.Suc)
        {
            //开始游戏事件
            NetworkPlayer.Instance.OnPlayingChange(true);

            //是观察者
            if (result.Watch)
            {
                //NetworkPlayer.Instance.OnStartGame(Chess.None);
            }
            //是玩家
            else
            {
                NetworkPlayer.Instance.playWithAI = false;
                NetworkPlayer.Instance.OnStartGame(result.Camp, result.enemyName);
            }
        }
        else
        {
            print("开始游戏失败");
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
