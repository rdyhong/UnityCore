using UnityEngine;
using System.Collections;

public class ObjectBase : MonoBehaviour
{
    public Transform cachedTransform
    {
        get
        {
            if (m_cachedTransform == null)
            {
                m_cachedTransform = transform;
            }
            return m_cachedTransform;
        }
    }
    private Transform m_cachedTransform;

    public GameObject cachedGameObject
    {
        get
        {
            if (m_cachedGameObject == null)
            {
                m_cachedGameObject = gameObject;
            }
            return m_cachedGameObject;
        }
    }
    private GameObject m_cachedGameObject;

    public RectTransform cachedRectTransform
    {
        get
        {
            if (m_cachedRectTransform == null)
            {
                m_cachedRectTransform = GetComponent<RectTransform>();
            }
            return m_cachedRectTransform;
        }
    }
    private RectTransform m_cachedRectTransform;

}
