using System;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// һ����Ϸ�ͻ���ֻ�ܴ���һ���������
/// </summary>
public class NetworkPlayer : MonoBehaviour
{
    //����
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
    public Hero hero;                    //��������
    [HideInInspector]
    public int RoomId = 0;                  //�������
    [HideInInspector]
    public bool Playing = false;            //������Ϸ
    [HideInInspector]
    public string Name;                     //����

    public Action<int> OnRoomIdChange;      //����ID�ı�
    public Action<bool> OnPlayingChange;    //��Ϸ״̬�ı�
    public Action<Hero.Camp, string> OnStartGame;  //��ʼ��Ϸ
    public Action<string> OnNameChange;     //���ָı�
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
