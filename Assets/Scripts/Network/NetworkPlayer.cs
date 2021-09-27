using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 一个游戏客户端只能存在一个网络玩家
/// </summary>
public class NetworkPlayer : MonoBehaviour
{
    //单例
    private NetworkPlayer() { }
    private static NetworkPlayer inst;
    public static NetworkPlayer Instance
    {
        get
        {
            if (inst == null)
            {
                GameObject go = new GameObject();
                inst = go.AddComponent<NetworkPlayer>();
                DontDestroyOnLoad(inst);
            }
            return inst;
        }
    }

    public Hero.Camp camp
    {
        get => hero.m_camp;
    }

    [HideInInspector]
    public Hero hero;                    //棋子类型
    [HideInInspector]
    public int RoomId = 0;                  //房间号码
    [HideInInspector]
    public bool Playing = false;            //正在游戏
    [HideInInspector]
    public string Name;                     //名字

    public Action<int> OnRoomIdChange;      //房间ID改变
    public Action<bool> OnPlayingChange;    //游戏状态改变
    public Action<Hero.Camp, string> OnStartGame;  //开始游戏
    public Action<string> OnNameChange;     //名字改变
    public string EnemyName;
    public bool playWithAI = true;
    private void Awake()
    {

        OnRoomIdChange += (roomId) => RoomId = roomId;

        OnPlayingChange += (playing) => Playing = playing;

        OnStartGame += (heroCamp, name) =>
        {
            EnemyName = name;
            hero = GameManager.Inst.InitPlayer(heroCamp);
            if(!playWithAI)
                SceneManager.LoadScene("Game");
        };

        OnNameChange += (name) => Name = name;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Playing)
        {
            //Network.Instance.PlayChessRequest(RoomId);
        }
    }
}
