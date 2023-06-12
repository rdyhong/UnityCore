using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ObjectPool Define
public partial class ObjectPool : Singleton<ObjectPool>
{
    //Pooling Object
    public class PoolObject : ObjectBase
    {
        public Object cachedObject;
        // Is Dynamic
        public bool isDynamicPoolObject;
    }

    public static bool isComplete = false;

    public Transform cachedObjectRecycleTargetTransform
    {
        get
        {
            if (m_cachedObjectRecycleTargetTransform == null)
            {
                Transform parnetObject = new GameObject("PoolParent").GetComponent<Transform>();
                m_cachedObjectRecycleTargetTransform = parnetObject;
                parnetObject.SetParent(transform);
                parnetObject.gameObject.SetActive(false);
            }

            return m_cachedObjectRecycleTargetTransform;
        }
    }
    private Transform m_cachedObjectRecycleTargetTransform;

    public Transform cachedObjectRecycleTargetTransformForDynamicCreatedObject
    {
        get
        {
            if (m_cachedObjectRecycleTargetTransformForDynamicCreatedObject == null)
            {
                Transform parnetObject = new GameObject("PoolParentForDynamicCreatedObject").GetComponent<Transform>();
                m_cachedObjectRecycleTargetTransformForDynamicCreatedObject = parnetObject;
                parnetObject.SetParent(transform);
                parnetObject.gameObject.SetActive(false);
            }

            return m_cachedObjectRecycleTargetTransformForDynamicCreatedObject;
        }
    }
    private Transform m_cachedObjectRecycleTargetTransformForDynamicCreatedObject;

    private Dictionary<string, GameObject> m_basePrefabDictionary = new Dictionary<string, GameObject>();

    public Dictionary<string, List<PoolObject>> totalObjectPoolDictionary = new Dictionary<string, List<PoolObject>>();
    public Dictionary<string, List<PoolObject>> currentNotUsingPoolDictionary = new Dictionary<string, List<PoolObject>>();
    public Dictionary<string, List<PoolObject>> currentUsingPoolDictionary = new Dictionary<string, List<PoolObject>>();

    private List<string> m_totalDynamicCreatedPoolNameList = new List<string>();


    public void CreatePoolInit()
    {
        DebugMgr.Log("Object Pool Init");

        // GameObject
        CreatePool<Projectile>("Assets/Rdyhong/02_Prefabs/01.Object/Projectile.prefab", 50);

        // UI
        CreatePool<Content_RoomListEle>("Assets/Rdyhong/02_Prefabs/02.UI/Content_RoomListEle.prefab", 20);
        CreatePool<Content_RoomPlayerInfo>("Assets/Rdyhong/02_Prefabs/02.UI/Content_RoomPlayerInfo.prefab", 20);

        isComplete = true;

        DebugMgr.Log("Complete ::: Create Pool");
    }

    private void CreatePool<T>(string name, int poolCount = 1, bool isDynamicPool = false) where T : UnityEngine.Object
    {
        try
        {
            GameObject resource = AddressableMgr.loadedGameObj[name];

            if (resource == null)
                DebugMgr.Log("Not Exists ::: " + name);
            else
                ObjectPool.CreatePool<T>(resource, poolCount, isDynamicPool);
        }
        catch
        {
            DebugMgr.Log($"Fail to create pool.({name})");
        }
    }

    public static void CreatePool<T>(GameObject baseObject, int poolCount, bool isDynamicPool = false) where T : Object
    {
        Inst.createPool<T>(baseObject, poolCount, isDynamicPool);
    }

    public static T Spawn<T>(string poolName, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, Vector3.zero, Vector3.zero, Vector3.one, null, origin);
    }

    public static T Spawn<T>(string poolName, Vector3 localPosition, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, localPosition, Vector3.zero, Vector3.one, null, origin);
    }

    public static T Spawn<T>(string poolName, Vector3 localPosition, Transform parent, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, localPosition, Vector3.zero, Vector3.one, parent, origin);
    }

    public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, localPosition, localRotation, Vector3.one, null, origin);
    }

    public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Transform parent, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, localPosition, localRotation, Vector3.one, parent, origin);
    }

    public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, localPosition, localRotation, localScale, null, origin);
    }

    public static T Spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, Transform parent, bool origin = false) where T : Object
    {
        return Inst.spawn<T>(poolName, localPosition, localRotation, localScale, parent, origin);
    }

    public static void Recycle(Object targetObject)
    {
        Inst.recycle(targetObject);
    }

    public static bool Exist(string poolName)
    {
        return Inst.totalObjectPoolDictionary.ContainsKey(poolName);
    }

    public static void Clear(params string[] ignoreObjectPoolNames)
    {
        Inst.clear(ignoreObjectPoolNames);
    }

    public static void ClearTargetPools(params string[] targetObjectPoolNames)
    {
        Inst.clearTargetPools(targetObjectPoolNames);
    }

    public static void DestroyDynamicPools()
    {
        Inst.destroyDynamicPools();
    }
}
