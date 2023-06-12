using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static object syncObject = new object();
    private static T instance = null;

    public static T Inst
    {
        get
        {
            if (instance == null)
            {
                lock (syncObject)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        instance = obj.AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (null == instance)
        {
            instance = this as T;
        }

        SetParent();

        DebugMgr.Log($"Manager Awake ::: {this.name}");
    }
    protected virtual void OnDestroyedInit()
    {
        var obj = GameObject.Find("DontDestoryObject").transform;
        if (obj != null)
            transform.SetParent(obj);
    }

    void SetParent()
    {
        string parentName = "Managers";
        Transform _parent = GameObject.Find(parentName)?.transform;

        if(_parent == null)
        {
            _parent = new GameObject(parentName).transform;
            DontDestroyOnLoad(_parent);
        }

        if(transform.parent != _parent)
        {
            transform.parent = _parent;
        }
    }
}
