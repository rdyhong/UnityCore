using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMgr
{
    public static void Log(string _m)
    {
#if Dev
        Debug.Log(_m);
#endif
    }
    public static void Log(int _m)
    {
#if Dev
        Debug.Log(_m);
#endif
    }
    public static void Log(float _m)
    {
#if Dev
        Debug.Log(_m);
#endif
    }
    public static void Log(object _m)
    {
#if Dev
        Debug.Log(_m);
#endif
    }

    // Log Error
    public static void LogErr(string _m)
    {
#if Dev
        Debug.LogError(_m);
#endif
    }
    public static void LogErr(int _m)
    {
#if Dev
        Debug.LogError(_m);
#endif
    }
    public static void LogErr(float _m)
    {
#if Dev
        Debug.LogError(_m);
#endif
    }
    public static void LogErr(object _m)
    {
#if Dev
        Debug.LogError(_m);
#endif
    }
}
