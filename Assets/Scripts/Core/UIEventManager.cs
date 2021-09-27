using Multiplay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIEventManager : MonoBehaviour
{
    public Text myName;
    public Text enemyName;
    public Button btnDraw;
    public Button btnEndTurn;
    public Button btnExit;
    public CardView bigOne;
    public CardView showOne;
    public void Awake()
    {

        if (NetworkPlayer.Instance.playWithAI)
            NetworkPlayer.Instance.OnStartGame.Invoke(Hero.Camp.p1, "人机");


        myName.text = NetworkPlayer.Instance.camp + NetworkPlayer.Instance.Name;
        enemyName.text = (NetworkPlayer.Instance.hero.m_camp == Hero.Camp.p1 ? Hero.Camp.p2 : Hero.Camp.p1) + NetworkPlayer.Instance.EnemyName;
        btnDraw.onClick.AddListener(_Draw);
        btnEndTurn.onClick.AddListener(Network.Instance.TurnNextRequest);


        if (NetworkPlayer.Instance.camp == Hero.Camp.p1)
        {
            btnEndTurn.interactable = true;
            btnEndTurn.transform.GetChild(0).GetComponent<Text>().text = "结束回合";
        }

        if (NetworkPlayer.Instance.camp == Hero.Camp.p2)
        {
            btnEndTurn.interactable = false;
            btnEndTurn.transform.GetChild(0).GetComponent<Text>().text = "对手回合";
        }

        NetworkPlayer.Instance.hero.onTurnBegan += () =>
        {
            btnEndTurn.interactable = true;
            btnEndTurn.transform.GetChild(0).GetComponent<Text>().text = "结束回合";
        };

        NetworkPlayer.Instance.hero.onTurnEnded += () =>
        {
            if (NetworkPlayer.Instance.playWithAI) return;
            btnEndTurn.interactable = false;
            btnEndTurn.transform.GetChild(0).GetComponent<Text>().text = "对手回合";
        };

        btnExit.onClick.AddListener(_Exit);


    }

    private void _Exit()
    {
        Network.Instance.ExitRoomRequest(8848);
        SceneManager.LoadScene("Main");
    }

    private void _Draw()
    {
        Network.Instance.DrawCardRequest(6, NetworkPlayer.Instance.camp);
    }

    public void ShowBig(bool toggle, Card card = null)
    {
        if (card != null) bigOne.SetModel(card);
        bigOne.gameObject.SetActive(toggle);
    }

    public void ShowShow(bool toggle, Card card)
    {
        showOne.SetModel(card);
        showOne.gameObject.SetActive(toggle);
    }

    public static void CreatRoomRequest(int roomId)
    {
        CreatRoom request = new CreatRoom();
        request.RoomId = roomId;
        byte[] data = NetworkUtils.Serialize(request);
        NetworkClient.Enqueue(MessageType.CreatRoom, data);
    }
}
