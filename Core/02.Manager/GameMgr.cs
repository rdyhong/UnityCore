//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : Singleton<GameMgr>
{
    static string s_version;
    public static string s_Version => s_version;

    protected override void Awake()
    {
        base.Awake();
    }

    // Application setting
    public void ApplicationSetting()
    {
        //Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        UnityEngine.Random.InitState(DateTime.Now.TimeOfDay.Seconds);
        s_version = Application.version;
        Application.runInBackground = false;
    }

}
