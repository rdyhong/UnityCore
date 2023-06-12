//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : Singleton<GameMgr>
{
    [HideInInspector] public static string version;

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
        version = Application.version;
        Application.runInBackground = true;
    }

}
