using UnityEngine;

public class Arrow : MonoBehaviour
{
    Vector3 startPoint;
    [SerializeField]
    bool bShow = false;
    static Arrow inst;
    public static Arrow Inst
    {
        get
        {
            if (inst == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/Arrow");
                GameObject go = GameObject.Instantiate(prefab, CardDB.inst.GetCanvas());
                go.SetActive(false);
                inst = go.GetComponent<Arrow>();
                DontDestroyOnLoad(inst);
            }
            return inst;
        }
    }

    private void Awake()
    {
        if (inst == null)
            inst = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        //if(bShow)
        {
            Vector3 prePoint = startPoint;
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector2 endPoint = Input.mousePosition;
                Transform trans = transform.GetChild(i);
                trans.position = Vector2.Lerp(startPoint, endPoint, 0.2f * i);

                //float dist = Vector3.Distance(trans.position, startPoint);
                //trans.gameObject.SetActive(i > 0 || dist > 50);
                //prePoint = trans.position;
            }

            if (transform.childCount > 0)
            {
                var end = transform.GetChild(transform.childCount - 1).gameObject;
                Vector3 direction = Input.mousePosition - startPoint;
                Vector3 vec = Vector3.forward * (direction.normalized.y) * 90;
                var rot = Quaternion.Euler(vec);
                end.transform.localRotation = rot;
            }
        }
    }

    public void Show(Vector3 start)
    {
        startPoint = start;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
