using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ResourceMgr : Singleton<ResourceMgr>
{
    public static bool loadComplete = false;

    private Action completeCallback;
    public void Init(Action _cmpleteCallback = null)
    {
        completeCallback = _cmpleteCallback;

        StartCoroutine(LoadCycle());
    }

    IEnumerator LoadCycle()
    {
        AddressableMgr.Inst.LoadInit();
        yield return new WaitUntil(() => AddressableMgr.isLoadComplete);

        ObjectPool.Inst.CreatePoolInit();
        yield return new WaitUntil(() => ObjectPool.isComplete);

        completeCallback?.Invoke();
    }

    public Sprite LoadSprite(string fileName , string filePath = "")
    {
        return Resources.Load<Sprite>(Path.Combine(filePath, fileName)) as Sprite;
    }

    public T LoadResource<T>(string fileName, string filePath ="") where T : UnityEngine.Object
    {
        var temp = Resources.Load<T>($"{filePath}/{fileName}") as T;
        if (temp == null)
            Debug.Log($"{filePath}/{fileName} is null !!");
        return Resources.Load<T>($"{filePath}/{fileName}") as T;
    }
}
