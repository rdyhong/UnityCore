using System;
using UnityEngine;

public static class TimeMgr
{
    public static float ObjTimeScale = 1;
    public static float ObjDeltaTime
    {
        get
        {
            return Time.deltaTime * ObjTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                ObjTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }
            ObjTimeScale = value;
            DebugUtil.Log($"Obj DeltaTime Set ::: {ObjTimeScale}");
        }
    }

    public static float ObjFixedTimeScale = 1;
    public static float ObjFixedDeltaTime
    {
        get
        {
            return Time.fixedDeltaTime * ObjFixedTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                ObjFixedTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }
            ObjFixedTimeScale = value;
            DebugUtil.Log($"Obj DeltaTime Set ::: {ObjFixedTimeScale}");
        }
    }

    private static float s_uiTimeScale = 1;
    public static float UIDeltaTime
    {
        get
        {
            return Time.deltaTime * s_uiTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                s_uiTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }

            s_uiTimeScale = value;
            DebugUtil.Log($"UIDeltaTime Set ::: {s_uiTimeScale}");
        }
    }

    public static double GetUtcTimeSeconds()
    {
        return (DateTime.UtcNow - DateTime.UnixEpoch).TotalSeconds;
    }
}
