using UnityEngine;

public class TimeMgr
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
