using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Object Pool Algorithm
public partial class ObjectPool : Singleton<ObjectPool>
{
    private void createPool<T>(GameObject baseObject, int poolCount, bool isDynamicPool = false) where T : Object
    {
        string objectName = baseObject.name;

        // Parent Transform
        Transform recycleTargetParent = null;

        if (m_basePrefabDictionary.ContainsKey(objectName) == false)
        {
            m_basePrefabDictionary.Add(objectName, baseObject);
        }

        // Set Parent Transform
        if (isDynamicPool == true)
        {
            recycleTargetParent = cachedObjectRecycleTargetTransformForDynamicCreatedObject;
            m_totalDynamicCreatedPoolNameList.Add(objectName);
        }
        else
        {
            recycleTargetParent = cachedObjectRecycleTargetTransform;
        }

        // Create Pool Dictonary
        if (totalObjectPoolDictionary.ContainsKey(objectName) == false)
        {
            totalObjectPoolDictionary.Add(objectName, new List<PoolObject>());
            currentNotUsingPoolDictionary.Add(objectName, new List<PoolObject>());
            currentUsingPoolDictionary.Add(objectName, new List<PoolObject>());
        }

        List<PoolObject> poolObjectList = totalObjectPoolDictionary[objectName];
        List<PoolObject> currentNotUsingpoolObject = currentNotUsingPoolDictionary[objectName];

        for (int i = 0; i < poolCount; i++)
        {
            PoolObject createdObject = Object.Instantiate<GameObject>(baseObject).AddComponent<PoolObject>();
            if (typeof(T) == createdObject.gameObject.GetType())
                createdObject.cachedObject = createdObject.cachedGameObject;
            else
                createdObject.cachedObject = createdObject.GetComponent<T>();
            createdObject.name = objectName;
            createdObject.isDynamicPoolObject = isDynamicPool;
            createdObject.cachedTransform.SetParent(recycleTargetParent);
            poolObjectList.Add(createdObject);
            currentNotUsingpoolObject.Add(createdObject);
        }
    }

    private T spawn<T>(string poolName, Vector3 localPosition, Vector3 localRotation, Vector3 localScale, Transform parent, bool origin = false) where T : Object
    {
        PoolObject poolObject = null;

        if (!totalObjectPoolDictionary.ContainsKey(poolName))
        {
#if UNITY_EDITOR
            Debug.LogError("This object isn't created. Need to call 'CreatePool()' method.\nPool name : " + poolName);
#endif
            return null;
        }

        List<PoolObject> poolList = currentNotUsingPoolDictionary[poolName];

        if (poolList.Count > 1)
        {
            poolObject = poolList[0];
            poolList.Remove(poolObject);
        }
        else
        {
            PoolObject createdPoolObject = Object.Instantiate(m_basePrefabDictionary[poolName]).AddComponent<PoolObject>();
            if (typeof(T) == createdPoolObject.gameObject.GetType())
                createdPoolObject.cachedObject = createdPoolObject.cachedGameObject;
            else
                createdPoolObject.cachedObject = createdPoolObject.GetComponent<T>();
            createdPoolObject.name = poolName;
            poolObject = createdPoolObject;
#if UNITY_EDITOR
            Debug.LogWarning("This object is created from 'Spawn()'.\nPool name : " + poolName);
#endif
        }

        Transform objectAsTransform = poolObject.cachedTransform;
        GameObject objectAsGameObject = poolObject.cachedGameObject;
        objectAsTransform.SetParent(parent);
        objectAsTransform.localPosition = localPosition;
        if (origin)
        {
            objectAsTransform.localEulerAngles = poolObject.cachedTransform.localEulerAngles;
            objectAsTransform.localScale = poolObject.cachedTransform.localScale;
        }
        else
        {
            objectAsTransform.localEulerAngles = localRotation;
            objectAsTransform.localScale = localScale;
        }

        if (objectAsGameObject.activeSelf == false)
            objectAsGameObject.SetActive(true);

        currentUsingPoolDictionary[poolName].Add(poolObject);

        return poolObject != null ? poolObject.cachedObject as T : null;
    }

    private void recycle(Object targetObject)
    {
        if (targetObject == null)
            return;

        string poolName = targetObject.name;
        if (!currentUsingPoolDictionary.ContainsKey(poolName))
        {
            Debug.LogWarning(poolName + " is Not Contained");
            return;
        }

        List<PoolObject> currentUsingPoolList = currentUsingPoolDictionary[poolName];

        PoolObject targetRecycleObject = null;

        for (int i = 0; i < currentUsingPoolList.Count; i++)
        {
            if (currentUsingPoolList[i].cachedObject == targetObject)
            {
                targetRecycleObject = currentUsingPoolList[i];
            }
        }

        if (targetRecycleObject != null)
        {
            Transform recycleTargetParent = null;

            if (targetRecycleObject.isDynamicPoolObject == true)
                recycleTargetParent = cachedObjectRecycleTargetTransformForDynamicCreatedObject;
            else
                recycleTargetParent = cachedObjectRecycleTargetTransform;

            targetRecycleObject.cachedTransform.SetParent(recycleTargetParent);
            targetRecycleObject.cachedTransform.localPosition = Vector3.zero;
            currentUsingPoolList.Remove(targetRecycleObject);
            currentNotUsingPoolDictionary[poolName].Add(targetRecycleObject);
        }
    }

    private void clear(params string[] ignoreObjectPoolNames)
    {
        List<PoolObject> totalTargetClearPoolObjectList = new List<PoolObject>();
        foreach (KeyValuePair<string, List<PoolObject>> item in currentUsingPoolDictionary)
        {
            bool isCanClear = true;

            for (int i = 0; i < ignoreObjectPoolNames.Length; i++)
            {
                if (ignoreObjectPoolNames[i].Equals(item.Key) == true)
                {
                    isCanClear = false;
                    break;
                }
            }

            if (isCanClear == true)
            {
                List<PoolObject> currentPoolObjects = item.Value;

                for (int i = 0; i < currentPoolObjects.Count; i++)
                {
                    totalTargetClearPoolObjectList.Add(currentPoolObjects[i]);
                }
            }
        }

        for (int i = 0; i < totalTargetClearPoolObjectList.Count; i++)
        {
            if (totalTargetClearPoolObjectList[i] != null)
            {
                string currentName = totalTargetClearPoolObjectList[i].name;
                if (currentUsingPoolDictionary.ContainsKey(currentName) == true)
                {
                    recycle(totalTargetClearPoolObjectList[i].cachedObject);
                }
            }
        }
    }

    private void clearTargetPools(params string[] targetObjectPoolNames)
    {
        List<PoolObject> totalTargetClearPoolObjectList = new List<PoolObject>();
        foreach (KeyValuePair<string, List<PoolObject>> item in currentUsingPoolDictionary)
        {
            bool isCanClear = false;

            for (int i = 0; i < targetObjectPoolNames.Length; i++)
            {
                if (targetObjectPoolNames[i].Equals(item.Key) == true)
                {
                    isCanClear = true;
                    break;
                }
            }

            if (isCanClear == true)
            {
                List<PoolObject> currentPoolObjects = item.Value;

                for (int i = 0; i < currentPoolObjects.Count; i++)
                {
                    totalTargetClearPoolObjectList.Add(currentPoolObjects[i]);
                }
            }
        }

        for (int i = 0; i < totalTargetClearPoolObjectList.Count; i++)
        {
            string currentName = totalTargetClearPoolObjectList[i].name;
            if (currentUsingPoolDictionary.ContainsKey(currentName) == true)
            {
                recycle(totalTargetClearPoolObjectList[i].cachedObject);
            }
        }
    }

    private void destroyTargetPools(params string[] targetObjectPoolNames)
    {
        List<PoolObject> totalTargetDestroyPoolObjectList = new List<PoolObject>();
        foreach (KeyValuePair<string, List<PoolObject>> item in currentUsingPoolDictionary)
        {
            bool isCanClear = false;

            for (int i = 0; i < targetObjectPoolNames.Length; i++)
            {
                if (targetObjectPoolNames[i].Equals(item.Key))
                {
                    isCanClear = true;
                    break;
                }
            }

            if (isCanClear == true)
            {
                List<PoolObject> currentPoolObjects = item.Value;

                for (int i = 0; i < currentPoolObjects.Count; i++)
                {
                    totalTargetDestroyPoolObjectList.Add(currentPoolObjects[i]);
                }
            }
        }

        for (int i = 0; i < totalTargetDestroyPoolObjectList.Count; i++)
        {
            string currentName = totalTargetDestroyPoolObjectList[i].name;
            if (currentUsingPoolDictionary.ContainsKey(currentName))
            {
                recycle(totalTargetDestroyPoolObjectList[i].cachedObject);
            }

            totalObjectPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
            currentUsingPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
            currentNotUsingPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
        }
    }

    private void destroyDynamicPools()
    {
        for (int i = 0; i < m_totalDynamicCreatedPoolNameList.Count; i++)
        {
            List<PoolObject> totalPoolList = totalObjectPoolDictionary[m_totalDynamicCreatedPoolNameList[i]];

            for (int j = 0; j < totalPoolList.Count; j++)
            {
                if (totalPoolList[j] != null)
                    DestroyImmediate(totalPoolList[j].gameObject);
            }

            List<PoolObject> currentUsingPoolList = currentUsingPoolDictionary[m_totalDynamicCreatedPoolNameList[i]];

            for (int j = 0; j < currentUsingPoolList.Count; j++)
            {
                if (currentUsingPoolList[j] != null)
                    DestroyImmediate(currentUsingPoolList[j].gameObject);
            }

            List<PoolObject> currentNotUsingPoolList = currentNotUsingPoolDictionary[m_totalDynamicCreatedPoolNameList[i]];

            for (int j = 0; j < currentNotUsingPoolList.Count; j++)
            {
                if (currentNotUsingPoolList[j] != null)
                    DestroyImmediate(currentNotUsingPoolList[j].gameObject);
            }

            totalObjectPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
            currentUsingPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
            currentNotUsingPoolDictionary.Remove(m_totalDynamicCreatedPoolNameList[i]);
        }
        m_totalDynamicCreatedPoolNameList.Clear();
    }
}
