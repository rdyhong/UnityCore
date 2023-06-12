using UnityEngine;

public class TimeMgr
{
    private static float objTimeScale = 1;
    public static float ObjDeltaTime
    {
        get
        {
            return Time.deltaTime * objTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                objTimeScale = 0f;
                DebugMgr.Log("Can't set time to lower than 0");
            }
            objTimeScale = value;
            DebugMgr.Log($"UIDeltaTime Set ::: {objTimeScale}");
        }
    }

    private static float uiTimeScale = 1;
    public static float UIDeltaTime
    {
        get
        {
            return Time.deltaTime * uiTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                uiTimeScale = 0f;
                DebugMgr.Log("Can't set time to lower than 0");
            }

            uiTimeScale = value;
            DebugMgr.Log($"UIDeltaTime Set ::: {uiTimeScale}");
        }
    }

}
