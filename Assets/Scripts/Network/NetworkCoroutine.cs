using System;
using UnityEngine;

public class NetworkCoroutine : MonoBehaviour
{
    public Action ApplicationQuitEvent;

    private static NetworkCoroutine _instance;

    // 场景单例(不随场景改变而销毁)
    public static NetworkCoroutine Instance
    {
        get
        {
            if (!_instance)
            {
                GameObject socketClientObj = new GameObject("NetworkCoroutine");
                _instance = socketClientObj.AddComponent<NetworkCoroutine>();
                DontDestroyOnLoad(socketClientObj);
            }
            return _instance;
        }
    }

    // 程序退出
    private void OnApplicationQuit()
    {
        if (ApplicationQuitEvent != null)
            ApplicationQuitEvent();
    }
}

