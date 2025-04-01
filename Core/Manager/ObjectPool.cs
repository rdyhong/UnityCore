using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolingObject
{
    public abstract void OnSpawn();
    public abstract void OnRecycle();
}

public class ObjectPool : Singleton<ObjectPool>
{
    private Transform _parent = null;
    public Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> _objPoolUsing = new Dictionary<string, List<GameObject>>();

    protected override void Awake()
    {
        base.Awake();

        _parent = new GameObject("WholePoolingObject").transform;
        _parent.SetParent(transform);
    }

    public T Spawn<T>(string path) where T : MonoBehaviour
    {
        GameObject go = null;

        // 풀 자체가 없으면
        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
            go = Instantiate(Resources.Load(path) as GameObject);
            go.transform.SetParent(_parent);
        }
        else // 풀이 있으면
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

        if (_objPoolUsing[path].Contains(obj))
        {
            _objPoolUsing[path].Remove(obj);
        }

        if (!_objPool.ContainsKey(path))
        {
            _objPool[path] = new List<GameObject>();
        }

        _objPool[path].Add(obj);
        obj.transform.SetParent(_parent);
    }

    // Assets
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
        foreach (string key in _objPoolUsing.Keys)
        {
            while (_objPoolUsing[key].Count > 0)
            {
                Recycle(_objPoolUsing[key][_objPoolUsing[key].Count - 1].gameObject);
            }
        }
    }
}
