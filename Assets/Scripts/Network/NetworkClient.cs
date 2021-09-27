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
    /// �ͻ�������״̬ö��
    /// </summary>
    private enum ClientState
    {
        None,        //δ����
        Connected,   //���ӳɹ�
    }

    //��Ϣ������ص��ֵ�
    private static Dictionary<MessageType, CallBack> _callBacks =
        new Dictionary<MessageType, CallBack>();
    //��������Ϣ����
    private static Queue<byte[]> _messages;
    //��ǰ״̬
    private static ClientState _curState;
    //�����������TCP���Ӳ���ȡ����ͨѶ��
    private static TcpClient _client;
    //������ͨѶ���ж�д����
    private static NetworkStream _stream;
    private static float _timer = 0;
    //Ŀ��ip
    private static IPAddress _address;
    //�˿ں�
    private static int _port;
    private static float HEARTBEAT_TIME = 5;
    public static bool Received = true;

    public static void Init(string address = null, int port = 8848)
    {
        //�����Ϻ����ظ�����
        if (_curState == ClientState.Connected)
            return;
        //���Ϊ����Ĭ�����ӱ���ip�ķ�����
        if (address == null)
            address = NetworkUtils.GetLocalIPv4();

        //���ͻ�ȡʧ����ȡ������
        if (!IPAddress.TryParse(address, out _address))
            return;
    }
    //ע��ص��¼�    
    public static void Register(MessageType type, CallBack method)
    {
        if (!_callBacks.ContainsKey(type))
            _callBacks.Add(type, method);
        else
            Debug.LogWarning("ע������ͬ�Ļص��¼�");
    }
    public static void Connect(string address = null, int port = 8848)
    {
        //�����Ϻ����ظ�����
        if (_curState == ClientState.Connected)
        {
            Debug.Log("�Ѿ������Ϸ�����");
            return;
        }
        if (address == null)
            address = NetworkUtils.GetLocalIPv4();

        //��ȡʧ����ȡ������
        if (!IPAddress.TryParse(address, out _address))
        {
            Debug.Log("IP��ַ����, �����³���");
            return;
        }

        _port = port;
        //���������������
        NetworkCoroutine.Instance.StartCoroutine(_Connect()); //(����ip���˿ںųɹ�����֤�����������ɹ�)
    }

    private static IEnumerator _Connect()
    {
        _client = new TcpClient();

        //�첽���ӷ�����
        IAsyncResult async = _client.BeginConnect(_address, _port, null, null);
        while (!async.IsCompleted)
        {
            Debug.Log("���ӷ�������");
            yield return null;
        }
        //�����첽
        _client.EndConnect(async);
        //��ȡ������
        _stream = _client.GetStream();

        _curState = ClientState.Connected;
        _messages = new Queue<byte[]>();
        Debug.Log("���ӷ������ɹ�");

        //�����첽������Ϣ
        NetworkCoroutine.Instance.StartCoroutine(_Send());
        //�����첽������Ϣ
        NetworkCoroutine.Instance.StartCoroutine(_Receive());
        //�����˳��¼�
        NetworkCoroutine.Instance.ApplicationQuitEvent +=
           () => { _client.Close(); _curState = ClientState.None; };
    }
    private static IEnumerator _Send()
    {
        //����������Ϣ
        while (_curState == ClientState.Connected)
        {
            _timer += Time.deltaTime;
            //�д�������Ϣ
            if (_messages.Count > 0)
            {
                byte[] data = _messages.Dequeue();
                yield return _Write(data); //�Ժ��ʵ��
            }

            //����������(ÿ��һ��ʱ�������������������)
            if (_timer >= HEARTBEAT_TIME)
            {
                //���û���յ���һ�η��������Ļظ�
                if (!Received)
                {
                    _curState = ClientState.None;
                    Debug.Log("����������ʧ��,�Ͽ�����");
                    yield break;
                }
                _timer = 0;
                //��װ��Ϣ
                byte[] data = NetworkUtils._Pack(MessageType.HeartBeat);
                //������Ϣ
                yield return _Write(data);

                Debug.Log("�ѷ���������");
            }
            yield return null; //��ֹ��ѭ��
        }
    }
    private static IEnumerator _Receive()
    {
        //����������Ϣ
        while (_curState == ClientState.Connected)
        {
            //�������ݰ�����(��������ͻ�����Ҫ�ϸ���һ����Э���ƶ����ݰ�)
            byte[] data = new byte[4];

            int length;         //��Ϣ����
            MessageType type;   //����
            int receive = 0;    //���ճ���

            //�첽��ȡ
            IAsyncResult async = _stream.BeginRead(data, 0, data.Length, null, null);
            while (!async.IsCompleted)
            {
                yield return null;
            }
            //�쳣����
            try
            {
                receive = _stream.EndRead(async);
            }
            catch (Exception ex)
            {
                _curState = ClientState.None;
                Debug.Log("��Ϣ��ͷ����ʧ��:" + ex.Message);
                yield break;
            }
            if (receive < data.Length)
            {
                _curState = ClientState.None;
                Debug.Log("��Ϣ��ͷ����ʧ��");
                yield break;
            }

            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader binary = new BinaryReader(stream, Encoding.UTF8); //UTF-8��ʽ����
                try
                {
                    length = binary.ReadUInt16();
                    type = (MessageType)binary.ReadUInt16();
                }
                catch (Exception)
                {
                    _curState = ClientState.None;
                    Debug.Log("��Ϣ��ͷ����ʧ��");
                    yield break;
                }
            }

            //����а���
            if (length - 4 > 0)
            {
                data = new byte[length - 4];
                //�첽��ȡ
                async = _stream.BeginRead(data, 0, data.Length, null, null);
                while (!async.IsCompleted)
                {
                    yield return null;
                }
                //�쳣����
                try
                {
                    receive = _stream.EndRead(async);
                }
                catch (Exception ex)
                {
                    _curState = ClientState.None;
                    //Info.Instance.Print("��Ϣ��ͷ����ʧ��:" + ex.Message, true);
                    yield break;
                }
                if (receive < data.Length)
                {
                    _curState = ClientState.None;
                    //Info.Instance.Print("��Ϣ��ͷ����ʧ��", true);
                    yield break;
                }
            }
            //û�а���
            else
            {
                data = new byte[0];
                receive = 0;
            }

            if (_callBacks.ContainsKey(type))
            {
                //ִ�лص��¼�
                CallBack method = _callBacks[type];
                method(data);
            }
            else
            {
                Debug.Log("δע������͵Ļص��¼�");
            }
        }
    }
    private static IEnumerator _Write(byte[] data)
    {
        //�������������, �ͻ�����Ȼ���������Ϣ
        if (_curState != ClientState.Connected || _stream == null)
        {
            Debug.Log("�Ͽ�����");
            yield break;
        }

        //�첽������Ϣ
        IAsyncResult async = _stream.BeginWrite(data, 0, data.Length, null, null);
        while (!async.IsCompleted)
        {
            yield return null;
        }
        //�쳣����
        try
        {
            _stream.EndWrite(async);
        }
        catch (Exception ex)
        {
            _curState = ClientState.None;
            Debug.Log("�Ͽ�����" + ex.Message);
        }
    }
    public static void Enqueue(MessageType type, byte[] data = null)
    {
        byte[] bytes = NetworkUtils._Pack(type, data); // Pack�������������Ѿ�ʵ��

        if (_curState == ClientState.Connected)
        {
            //�������                                 
            _messages.Enqueue(bytes);
        }
    }
}

