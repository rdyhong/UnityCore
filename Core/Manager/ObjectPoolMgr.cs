using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolingObject
{
    void OnSpawn();
    void OnRecycle();
}

public class ObjectPoolMgr : Singleton<ObjectPoolMgr>, IPunPrefabPool
{
    private Transform _parent = null;
    public Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> _objPoolUsing = new Dictionary<string, List<GameObject>>();

    // Photon 전용 풀 (네트워크 오브젝트)
    private readonly Dictionary<string, Queue<GameObject>> _photonPool = new Dictionary<string, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        _parent = new GameObject("WholePoolingObject").transform;
        _parent.SetParent(transform);

        // PhotonNetwork에 이 풀을 등록
        PhotonNetwork.PrefabPool = this;
    }

    #region ===== 기존 로컬 풀 기능 =====
    public T Spawn<T>(string path) where T : MonoBehaviour
    {
        GameObject go = null;
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
            go = Instantiate(Resources.Load(path) as GameObject);
            go.transform.SetParent(_parent);
        }
        else
        {
            if (_objPool[path].Count == 0)
            {
                go = Instantiate(Resources.Load(path) as GameObject);
                go.transform.SetParent(_parent);
            }
            else
            {
                go = _objPool[path][_objPool[path].Count - 1];
                _objPool[path].RemoveAt(_objPool[path].Count - 1);
            }
        }

        if (!_objPoolUsing.ContainsKey(path))
        {
            _objPoolUsing[path] = new List<GameObject>();
        }
        _objPoolUsing[path].Add(go);
        go.name = path;
        go.GetComponent<IPoolingObject>().OnSpawn();
        go.SetActive(true);
        return go.GetComponent<T>();
    }

    public void Recycle(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
        obj.transform.position = new Vector3(0, 2000, 0);
        string path = obj.name;
        if (_objPoolUsing.ContainsKey(path) && _objPoolUsing[path].Contains(obj))
        {
            _objPoolUsing[path].Remove(obj);
        }
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
        }
        _objPool[path].Add(obj);

        var poolingObject = obj.GetComponent<IPoolingObject>();
        poolingObject?.OnRecycle();

        obj.transform.SetParent(_parent);
    }

    public void Preload(string path, int count)
    {
        GameObject go = null;
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
        }

        for(int i = 0; i < count; i++)
        {
            go = Instantiate(Resources.Load(path) as GameObject);
            go.transform.SetParent(_parent);
            go.name = path;
            go.SetActive(false);
        }
    }

    public void LoadAsset<T>(ref T v, string path) where T : Object
    {
        T result = Resources.Load<T>(path);
        if (result != null)
        {
            v = result;
        }
    }

    public T LoadAsset<T>(string path) where T : Object
    {
        T result = Resources.Load<T>(path);
        if (result != null)
        {
            return result;
        }
        return null;
    }

    public void RecycleAll()
    {
        var keysToProcess = new List<string>(_objPoolUsing.Keys);
        foreach (string key in keysToProcess)
        {
            var objectsToRecycle = new List<GameObject>(_objPoolUsing[key]);
            foreach (GameObject obj in objectsToRecycle)
            {
                Recycle(obj);
            }
        }
    }
    #endregion

    #region ===== Photon Instantiate 풀링 =====
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;

        if (_photonPool.TryGetValue(prefabId, out var queue) && queue.Count > 0)
        {
            obj = queue.Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(prefabId);
            obj = Object.Instantiate(prefab, position, rotation);
        }

        var poolingObj = obj.GetComponent<IPoolingObject>();
        poolingObj?.OnSpawn();

        return obj;
    }

    public void Destroy(GameObject gameObject)
    {
        if (gameObject == null) return;

        string prefabId = gameObject.name.Replace("(Clone)", "").Trim();
        if (!_photonPool.ContainsKey(prefabId))
            _photonPool[prefabId] = new Queue<GameObject>();

        gameObject.SetActive(false);
        gameObject.transform.SetParent(_parent);

        var poolingObj = gameObject.GetComponent<IPoolingObject>();
        poolingObj?.OnRecycle();

        _photonPool[prefabId].Enqueue(gameObject);
    }

    public void PreloadPhoton(string prefabId, int count)
    {
        if (!_photonPool.ContainsKey(prefabId))
            _photonPool[prefabId] = new Queue<GameObject>();

        GameObject prefab = Resources.Load<GameObject>(prefabId);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(_parent);
            _photonPool[prefabId].Enqueue(obj);
        }
    }
    #endregion
}
