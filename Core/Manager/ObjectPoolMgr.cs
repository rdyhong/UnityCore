using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IPoolingObject
{
    public abstract void OnSpawn();
    public abstract void OnRecycle();
}

public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
{
    private Transform _parent = null;
    public Dictionary<string, List<GameObject>> _objPool = new Dictionary<string, List<GameObject>>();
    public Dictionary<string, List<GameObject>> _objPoolUsing = new Dictionary<string, List<GameObject>>();

    // Addressables 캐시 (즉시 생성용)
    private Dictionary<string, GameObject> _addressablePrefabCache = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        _parent = new GameObject("WholePoolingObject").transform;
        _parent.SetParent(transform);
    }

    // 기존 Resources 방식 스폰
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

    // 새로운 Addressables 즉시 생성 함수
    public T SpawnAddressable<T>(string addressableKey) where T : class
    {
        GameObject go = null;

        // 캐시에서 프리팹 확인
        if (!_addressablePrefabCache.ContainsKey(addressableKey))
        {
            Debug.LogError($"Addressable prefab '{addressableKey}' is not cached. Use PreloadAddressable first.");
            return null;
        }

        GameObject prefab = _addressablePrefabCache[addressableKey];

        // 풀에서 확인
        if (!_objPool.ContainsKey(addressableKey))
        {
            _objPool[addressableKey] = new List<GameObject>();
            go = Instantiate(prefab);
            go.transform.SetParent(_parent);
        }
        else
        {
            if (_objPool[addressableKey].Count == 0)
            {
                go = Instantiate(prefab);
                go.transform.SetParent(_parent);
            }
            else
            {
                go = _objPool[addressableKey][_objPool[addressableKey].Count - 1];
                _objPool[addressableKey].RemoveAt(_objPool[addressableKey].Count - 1);
            }
        }

        // 사용 중인 목록에 추가
        if (!_objPoolUsing.ContainsKey(addressableKey))
        {
            _objPoolUsing[addressableKey] = new List<GameObject>();
        }
        _objPoolUsing[addressableKey].Add(go);

        go.name = addressableKey;

        var poolingObject = go.GetComponent<IPoolingObject>();
        poolingObject?.OnSpawn();

        go.SetActive(true);
        return go.GetComponent<T>();
    }

    // Addressable 프리팹 미리 로드 (게임 시작 시 호출)
    public void PreloadAddressable(string addressableKey)
    {
        if (_addressablePrefabCache.ContainsKey(addressableKey))
        {
            return; // 이미 로드됨
        }

        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        handle.Completed += (AsyncOperationHandle<GameObject> obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                _addressablePrefabCache[addressableKey] = obj.Result;
                Debug.Log($"Preloaded addressable: {addressableKey}");
            }
            else
            {
                Debug.LogError($"Failed to preload addressable: {addressableKey}");
            }
        };
    }

    // 여러 Addressable 프리팹 한번에 로드
    public void PreloadAddressables(params string[] addressableKeys)
    {
        foreach (string key in addressableKeys)
        {
            PreloadAddressable(key);
        }
    }

    // Default Local Group의 모든 Addressable 프리팹 미리 로드
    public void PreloadMonsterBody()
    {
        StartCoroutine(PreloadMonsterBodyCoroutine());
    }

    private IEnumerator PreloadMonsterBodyCoroutine()
    {
        // Addressable 카탈로그에서 모든 위치 정보 가져오기
        var catalogHandle = Addressables.LoadResourceLocationsAsync("body_monster");
        yield return catalogHandle;

        if (catalogHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var locations = catalogHandle.Result;
            Debug.Log($"Found {locations.Count} addressable assets in catalog");

            foreach (var location in locations)
            {
                // GameObject 타입만 필터링
                if (location.ResourceType == typeof(GameObject))
                {
                    string key = location.PrimaryKey;

                    // 이미 로드된 경우 스킵
                    if (!_addressablePrefabCache.ContainsKey(key))
                    {
                        PreloadAddressable(key);
                        Debug.Log($"Preloading: {key}");
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load addressable catalog");
        }

        Addressables.Release(catalogHandle);
    }

    // 특정 레이블의 모든 Addressable 프리팹 미리 로드
    public void PreloadByLabel(string label)
    {
        StartCoroutine(PreloadByLabelCoroutine(label));
    }

    private IEnumerator PreloadByLabelCoroutine(string label)
    {
        var handle = Addressables.LoadResourceLocationsAsync(label);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var locations = handle.Result;
            Debug.Log($"Found {locations.Count} assets with label '{label}'");

            foreach (var location in locations)
            {
                if (location.ResourceType == typeof(GameObject))
                {
                    string key = location.PrimaryKey;

                    if (!_addressablePrefabCache.ContainsKey(key))
                    {
                        PreloadAddressable(key);
                        Debug.Log($"Preloading by label '{label}': {key}");
                    }
                }
            }
        }
        else
        {
            Debug.LogError($"Failed to load assets with label '{label}'");
        }

        Addressables.Release(handle);
    }

    // 모든 GameObject 타입 Addressable 미리 로드 (전체)
    public void PreloadAllGameObjects()
    {
        StartCoroutine(PreloadAllGameObjectsCoroutine());
    }

    private IEnumerator PreloadAllGameObjectsCoroutine()
    {
        var handle = Addressables.LoadResourceLocationsAsync(typeof(GameObject));
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var locations = handle.Result;
            Debug.Log($"Found {locations.Count} GameObject addressables");

            foreach (var location in locations)
            {
                string key = location.PrimaryKey;

                if (!_addressablePrefabCache.ContainsKey(key))
                {
                    PreloadAddressable(key);
                    Debug.Log($"Preloading GameObject: {key}");
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load GameObject addressables");
        }

        Addressables.Release(handle);
    }

    // Addressable이 로드되었는지 확인
    public bool IsAddressableLoaded(string addressableKey)
    {
        return _addressablePrefabCache.ContainsKey(addressableKey);
    }

    // 기존 Recycle 함수 (그대로 유지)
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

    // 기존 LoadAsset 함수들 (그대로 유지)
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
}

public class PoolingUsageExample : MonoBehaviour
{
    void Start()
    {
        var poolMgr = ObjectPoolMgr.Inst;

        // 방법 1: 특정 키들 미리 로드
        poolMgr.PreloadAddressables("AddressableBullet", "AddressableEnemy", "AddressableEffect");

        // 방법 2: Default Local Group 전체 미리 로드
        poolMgr.PreloadMonsterBody();

        // 방법 3: 특정 레이블의 모든 에셋 미리 로드
        poolMgr.PreloadByLabel("Weapons");

        // 방법 4: 모든 GameObject 타입 Addressable 미리 로드
        // poolMgr.PreloadAllGameObjects();
    }

    void Update()
    {
        var poolMgr = ObjectPoolMgr.Inst;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 기존 Resources 방식
            var resourceBullet = poolMgr.Spawn<Bullet>("Prefabs/Bullet");

            // 새로운 Addressables 방식 (미리 로드된 경우만)
            if (poolMgr.IsAddressableLoaded("AddressableBullet"))
            {
                var addressableBullet = poolMgr.SpawnAddressable<Bullet>("AddressableBullet");
            }
        }
    }
}
