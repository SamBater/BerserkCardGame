using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaittingRoom : MonoBehaviour
{
    [SerializeField]
    private InputField _ipAddressIpt;     //服务器IP输入框
    [SerializeField]
    private InputField _roomIdIpt;        //房间号码输入框
    [SerializeField]
    private InputField _nameIpt;          //名字输入框

    [SerializeField]
    private Button _connectServerBtn;     //连接服务器按钮
    [SerializeField]
    private Button _enrollBtn;            //注册按钮
    [SerializeField]
    private Button _creatRoomBtn;         //创建房间按钮
    [SerializeField]
    private Button _enterRoomBtn;         //加入房间按钮
    [SerializeField]
    private Button _exitRoomBtn;          //退出房间按钮
    [SerializeField]
    private Button _startGameBtn;         //开始游戏按钮
    [SerializeField]
    private Button _btnAI;

    [SerializeField]
    private Text _gameStateTxt;           //游戏状态文本
    [SerializeField]
    private Text _roomIdTxt;              //房间号码文本
    [SerializeField]
    private Text _nameTxt;                //名字文本

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
