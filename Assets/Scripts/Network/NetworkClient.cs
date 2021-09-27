using Multiplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class NetworkClient
{
    public delegate void CallBack(byte[] bytes);
    /// <summary>
    /// 客户端网络状态枚举
    /// </summary>
    private enum ClientState
    {
        None,        //未连接
        Connected,   //连接成功
    }

    //消息类型与回调字典
    private static Dictionary<MessageType, CallBack> _callBacks =
        new Dictionary<MessageType, CallBack>();
    //待发送消息队列
    private static Queue<byte[]> _messages;
    //当前状态
    private static ClientState _curState;
    //向服务器建立TCP连接并获取网络通讯流
    private static TcpClient _client;
    //在网络通讯流中读写数据
    private static NetworkStream _stream;
    private static float _timer = 0;
    //目标ip
    private static IPAddress _address;
    //端口号
    private static int _port;
    private static float HEARTBEAT_TIME = 5;
    public static bool Received = true;

    public static void Init(string address = null, int port = 8848)
    {
        //连接上后不能重复连接
        if (_curState == ClientState.Connected)
            return;
        //如果为空则默认连接本机ip的服务器
        if (address == null)
            address = NetworkUtils.GetLocalIPv4();

        //类型获取失败则取消连接
        if (!IPAddress.TryParse(address, out _address))
            return;
    }
    //注册回调事件    
    public static void Register(MessageType type, CallBack method)
    {
        if (!_callBacks.ContainsKey(type))
            _callBacks.Add(type, method);
        else
            Debug.LogWarning("注册了相同的回调事件");
    }
    public static void Connect(string address = null, int port = 8848)
    {
        //连接上后不能重复连接
        if (_curState == ClientState.Connected)
        {
            Debug.Log("已经连接上服务器");
            return;
        }
        if (address == null)
            address = NetworkUtils.GetLocalIPv4();

        //获取失败则取消连接
        if (!IPAddress.TryParse(address, out _address))
        {
            Debug.Log("IP地址错误, 请重新尝试");
            return;
        }

        _port = port;
        //与服务器建立连接
        NetworkCoroutine.Instance.StartCoroutine(_Connect()); //(连接ip跟端口号成功不保证网络流建立成功)
    }

    private static IEnumerator _Connect()
    {
        _client = new TcpClient();

        //异步连接服务器
        IAsyncResult async = _client.BeginConnect(_address, _port, null, null);
        while (!async.IsCompleted)
        {
            Debug.Log("连接服务器中");
            yield return null;
        }
        //结束异步
        _client.EndConnect(async);
        //获取网络流
        _stream = _client.GetStream();

        _curState = ClientState.Connected;
        _messages = new Queue<byte[]>();
        Debug.Log("连接服务器成功");

        //设置异步发送消息
        NetworkCoroutine.Instance.StartCoroutine(_Send());
        //设置异步接收消息
        NetworkCoroutine.Instance.StartCoroutine(_Receive());
        //设置退出事件
        NetworkCoroutine.Instance.ApplicationQuitEvent +=
           () => { _client.Close(); _curState = ClientState.None; };
    }
    private static IEnumerator _Send()
    {
        //持续发送消息
        while (_curState == ClientState.Connected)
        {
            _timer += Time.deltaTime;
            //有待发送消息
            if (_messages.Count > 0)
            {
                byte[] data = _messages.Dequeue();
                yield return _Write(data); //稍后会实现
            }

            //心跳包机制(每隔一段时间向服务器发送心跳包)
            if (_timer >= HEARTBEAT_TIME)
            {
                //如果没有收到上一次发心跳包的回复
                if (!Received)
                {
                    _curState = ClientState.None;
                    Debug.Log("心跳包接受失败,断开连接");
                    yield break;
                }
                _timer = 0;
                //封装消息
                byte[] data = NetworkUtils._Pack(MessageType.HeartBeat);
                //发送消息
                yield return _Write(data);

                Debug.Log("已发送心跳包");
            }
            yield return null; //防止死循环
        }
    }
    private static IEnumerator _Receive()
    {
        //持续接受消息
        while (_curState == ClientState.Connected)
        {
            //解析数据包过程(服务器与客户端需要严格按照一定的协议制定数据包)
            byte[] data = new byte[4];

            int length;         //消息长度
            MessageType type;   //类型
            int receive = 0;    //接收长度

            //异步读取
            IAsyncResult async = _stream.BeginRead(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //异常处理
            try
            {
                receive = _stream.EndRead(async);
            }
            catch (Exception ex)
            {
                _curState = ClientState.None;
                Debug.Log("消息包头接收失败:" + ex.Message);
                yield break;
            }
            if (receive < data.Length)
            {
                _curState = ClientState.None;
                Debug.Log("消息包头接收失败");
                yield break;
            }

            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader binary = new BinaryReader(stream, Encoding.UTF8); //UTF-8格式解析
                try
                {
                    length = binary.ReadUInt16();
                    type = (MessageType)binary.ReadUInt16();
                }
                catch (Exception)
                {
                    _curState = ClientState.None;
                    Debug.Log("消息包头接收失败");
                    yield break;
                }
            }

            //如果有包体
            if (length - 4 > 0)
            {
                data = new byte[length - 4];
                //异步读取
                async = _stream.BeginRead(data, 0, data.Length, null, null);
                while (!async.IsCompleted)
                {
                    yield return null;
                }
                //异常处理
                try
                {
                    receive = _stream.EndRead(async);
                }
                catch (Exception ex)
                {
                    _curState = ClientState.None;
                    //Info.Instance.Print("消息包头接收失败:" + ex.Message, true);
                    yield break;
                }
                if (receive < data.Length)
                {
                    _curState = ClientState.None;
                    //Info.Instance.Print("消息包头接收失败", true);
                    yield break;
                }
            }
            //没有包体
            else
            {
                data = new byte[0];
                receive = 0;
            }

            if (_callBacks.ContainsKey(type))
            {
                //执行回调事件
                CallBack method = _callBacks[type];
                method(data);
            }
            else
            {
                Debug.Log("未注册该类型的回调事件");
            }
        }
    }
    private static IEnumerator _Write(byte[] data)
    {
        //如果服务器下线, 客户端依然会继续发消息
        if (_curState != ClientState.Connected || _stream == null)
        {
            Debug.Log("断开连接");
            yield break;
        }

        //异步发送消息
        IAsyncResult async = _stream.BeginWrite(data, 0, data.Length, null, null);
        while (!async.IsCompleted)
        {
            yield return null;
        }
        //异常处理
        try
        {
            _stream.EndWrite(async);
        }
        catch (Exception ex)
        {
            _curState = ClientState.None;
            Debug.Log("断开连接" + ex.Message);
        }
    }
    public static void Enqueue(MessageType type, byte[] data = null)
    {
        byte[] bytes = NetworkUtils._Pack(type, data); // Pack方法在上文中已经实现

        if (_curState == ClientState.Connected)
        {
            //加入队列                                 
            _messages.Enqueue(bytes);
        }
    }
}

