using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TopText : MonoBehaviour
{
    [Range(4, 8)]
    public float maxShowTime = 5f;
    [Range(1, 5)]
    public float fadeTime = 1.5f;
    public Text txt;
    public static TopText inst;
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            txt = GetComponent<Text>();
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    public void ShowTopText(string msg)
    {
        txt.text = msg;
        StartCoroutine("ShowMsg");
    }

    IEnumerator ShowMsg()
    {
        Color originColor = txt.color;
        float a = originColor.a;
        float timer = 0;
        while (timer < maxShowTime)
        {
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        timer = 0;

        while (timer < fadeTime)
        {
            timer += 0.1f;
            a -= Time.deltaTime * (0.1f / fadeTime) * 255;
            txt.color = new Color(originColor.r, originColor.g, originColor.b, a);
            yield return new WaitForSeconds(0.05f);
        }

        txt.text = null;
        txt.color = new Color(originColor.r, originColor.g, originColor.b, 255);
        yield return null;
    }
}
