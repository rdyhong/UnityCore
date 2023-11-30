using UnityEngine;

public class TimeMgr
{
    private static float s_objTimeScale = 1;
    public static float ObjDeltaTime
    {
        get
        {
            return Time.deltaTime * s_objTimeScale;
        }
        set
        {
            if (value <= 0)
            {
                s_objTimeScale = 0f;
                DebugUtil.Log("Can't set time to lower than 0");
            }
            s_objTimeScale = value;
            DebugUtil.Log($"UIDeltaTime Set ::: {s_objTimeScale}");
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

}
