using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaittingRoom : MonoBehaviour
{
    [SerializeField]
    private InputField _ipAddressIpt;     //������IP�����
    [SerializeField]
    private InputField _roomIdIpt;        //������������
    [SerializeField]
    private InputField _nameIpt;          //���������

    [SerializeField]
    private Button _connectServerBtn;     //���ӷ�������ť
    [SerializeField]
    private Button _enrollBtn;            //ע�ᰴť
    [SerializeField]
    private Button _creatRoomBtn;         //�������䰴ť
    [SerializeField]
    private Button _enterRoomBtn;         //���뷿�䰴ť
    [SerializeField]
    private Button _exitRoomBtn;          //�˳����䰴ť
    [SerializeField]
    private Button _startGameBtn;         //��ʼ��Ϸ��ť
    [SerializeField]
    private Button _btnAI;

    [SerializeField]
    private Text _gameStateTxt;           //��Ϸ״̬�ı�
    [SerializeField]
    private Text _roomIdTxt;              //��������ı�
    [SerializeField]
    private Text _nameTxt;                //�����ı�

    void Start()
    {
        _connectServerBtn.onClick.AddListener(_ConnectServerBtn);
        _enrollBtn.onClick.AddListener(_EnrollBtn);
        _creatRoomBtn.onClick.AddListener(_CreatRoomBtn);
        _enterRoomBtn.onClick.AddListener(_EnterRoomBtn);
        _exitRoomBtn.onClick.AddListener(_ExitRoomBtn);
        _startGameBtn.onClick.AddListener(_StartGameBtn);
        _btnAI.onClick.AddListener(_PlayWithAI);
    }

    public void _PlayWithAI()
    {
        NetworkPlayer.Instance.playWithAI = true;
        SceneManager.LoadScene("Game");
    }

    private void _StartGameBtn()
    {
        Network.Instance.StartGameRequest(8848);
    }

    private void _ExitRoomBtn()
    {
        Network.Instance.ExitRoomRequest(NetworkPlayer.Instance.RoomId);
    }

    private void _EnterRoomBtn()
    {
        Network.Instance.EnterRoomRequest(8848);
    }

    private void _CreatRoomBtn()
    {
        Network.Instance.CreatRoomRequest(8848);
    }

    private void _EnrollBtn()
    {
        Network.Instance.EnrollRequest(_nameIpt.text);
    }

    private void _ConnectServerBtn()
    {
        NetworkClient.Init();
        NetworkClient.Connect();
    }
}
