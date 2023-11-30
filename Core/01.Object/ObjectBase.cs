using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour
{
    public Transform CachedTf
    {
        get
        {
            if (_cachedTf == null)
            {
                _cachedTf = transform;
            }
            return _cachedTf;
        }
    }
    private Transform _cachedTf;

    public GameObject CachedGo
    {
        get
        {
            if (CachedGO == null)
            {
                CachedGO = gameObject;
            }
            return CachedGO;
        }
    }
    private GameObject CachedGO;

    public RectTransform CachedRectTf
    {
        get
        {
            if (_cachedRectTf == null)
            {
                _cachedRectTf = GetComponent<RectTransform>();
            }
            return _cachedRectTf;
        }
    }
    private RectTransform _cachedRectTf;

}
