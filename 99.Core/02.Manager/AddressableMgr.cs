using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

/// <summary>
/// Addressable Load, Manage loaded GameObject
/// </summary>
public class AddressableMgr : Singleton<AddressableMgr>
{
    // Loaded GameObject Map ( key == address )
    public static Dictionary<string, GameObject> loadedGameObj = new Dictionary<string, GameObject>();

    // Check Load Complete
    public static bool isLoadComplete = false;

    private Queue<Action> queue = new Queue<Action>();

    private List<string> onLoadingAddress = new List<string>();

    private int m_totalLoadCount = 0;
    private int m_curLoadedCount = 0;

    private bool ready = true;
    
    IEnumerator LoadQueueCo()
    {
        while(true)
        {
            yield return null;
            
            if (queue.Count > 0 && ready)
            {
                DebugMgr.Log($"Addressable Queue Left ::: {queue.Count}");

                ready = false;
                queue.Dequeue()?.Invoke();
            }

            //if (isLoadComplete)
            //    yield break;
        }
    }

    public void LoadInit()
    {
        DebugMgr.Log("Addressable Init");

        // Use load method under here

        // Cached For ObjectPool

        // GameObject
        AddLoadQueue("Assets/Rdyhong/02_Prefabs/01.Object/Projectile.prefab");

        // UI Prefab
        AddLoadQueue("Assets/Rdyhong/02_Prefabs/02.UI/Content_RoomListEle.prefab");
        AddLoadQueue("Assets/Rdyhong/02_Prefabs/02.UI/Content_RoomPlayerInfo.prefab");

        // Load UI Prefab

        // Start Load Coroutine
        StartCoroutine(LoadQueueCo());
    }

    void AddLoadQueue(string address, Action _cb = null)
    {
        m_totalLoadCount++;

        queue.Enqueue(() => LoadPoolObject(address, _cb));
    }

    void LoadPoolObject(string address, Action _cb = null, bool isUI = false)
    {
        // If already loading or loaded
        if (loadedGameObj.ContainsKey(address) || onLoadingAddress.Contains(address))
        {
            m_totalLoadCount--;
            ready = true;
            return;
        }

        // Temporary storage the on Loading Address
        onLoadingAddress.Add(address);

        Addressables.InstantiateAsync(address, new Vector3(0, -500, 0), Quaternion.identity).Completed += handle =>
        {
            onLoadingAddress.Remove(address);

            m_curLoadedCount++;

            loadedGameObj[address] = handle.Result;
            loadedGameObj[address].transform.parent = transform;
            loadedGameObj[address].SetActive(false);
            loadedGameObj[address].name = loadedGameObj[address].name.Replace("(Clone)", string.Empty);

            ready = true;

            _cb?.Invoke();

            if (Inst.m_totalLoadCount == Inst.m_curLoadedCount)
            {
                isLoadComplete = true;
                DebugMgr.Log("Complete ::: Addressable");
            }
        };
    }

    public void InstantiateObj<T>(string address, Action<T> _callBack = null)
    {
        Addressables.InstantiateAsync(address).Completed += handle =>
        {
            _callBack?.Invoke(handle.Result.GetComponent<T>());
        };
    }

    public static void LoadAsset<T>(string address, T _ref, Action _callBack =  null)
    {
        //Addressables.InstantiateAsync(address);
        Addressables.LoadAssetAsync<T>(address).Completed += handle => {
            //result = handle.Result;
            _ref = handle.Result;
            _callBack?.Invoke();
        };
    }
}
