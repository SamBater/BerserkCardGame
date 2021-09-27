using System;
using System.Collections.Generic;

namespace Multiplay
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        None,         //空类型
        HeartBeat,    //心跳包验证

        //以下为玩家操作请求类型
        Enroll,       //注册
        CreatRoom,    //创建房间
        EnterRoom,    //进入房间
        ExitRoom,     //退出房间
        StartGame,    //开始游戏
        PlayCard,     //玩牌
        DrawCard,     //抽卡
        PlayMinion,   //部署随从
        TurnNext,     //更换回合
        Attack        //攻击
    }

    [Serializable]
    public class Enroll
    {
        public string Name;//姓名

        public bool Suc;   //是否成功
    }

    [Serializable]
    public class CreatRoom
    {
        public int RoomId; //房间号码

        public bool Suc;   //是否成功
    }

    [Serializable]
    public class EnterRoom
    {
        public int RoomId;      //房间号码

        public Result result;   //结果
        public enum Result
        {
            None,
            Player,
            Observer,
        }
    }

    [Serializable]
    public class ExitRoom
    {
        public int RoomId;  //房间号码

        public bool Suc;    //是否成功
    }

    [Serializable]
    public class StartGame
    {
        public int RoomId;            //房间号码
        public bool Suc;              //是否成功
        public Hero.Camp Camp;      //阵营
        public bool Watch;            //是否是观察者
        public string enemyName;
    }

    /// <summary>
    /// 发生于：玩家使用了一张卡牌
    /// </summary>
    [Serializable]
    public class PlayCard
    {
        public int RoomId;       //房间号码
        public Hero.Camp camp;
        public Card card;
        public int handIndex;    //在手中的顺序
        public bool Suc;         //操作结果
    }

    /// <summary>
    /// 玩家抽牌
    /// </summary>
    [Serializable]
    public class DrawCard
    {
        public int RoomId;
        public Hero.Camp camp;
        public Card card;
        public bool suc;
    }

    /// <summary>
    /// 玩家下了一只随从
    /// </summary>
    [Serializable]
    public class PlayMinion
    {
        public int RoomId;
        public Hero.Camp camp;
        public Card card;
        public int targetindex;  //在场地上的位置
        public bool Suc;         //操作结果
    }

    /// <summary>
    /// 玩家结束回合
    /// </summary>
    [Serializable]
    public class TurnNext
    {
        public int Roomid;
        public bool Suc;
    }

    /// <summary>
    /// 攻击
    /// </summary>
    [Serializable]
    public class Attack
    {
        public int Roomid;
        public bool Suc;
        public Hero.Camp StarterCmp;
        public int starterIndex;
        public int targetIndex;
    }
}
