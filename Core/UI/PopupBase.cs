using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupBase : MonoBehaviour
{
    [SerializeField] private Transform _rootTf;
    private Sequence openSeq;
    private Sequence closeSeq;

    private float _animDuration = 0.3f;

    public virtual void Open(bool withAnim = true)
    {
        if (openSeq != null) openSeq.Kill();
        _rootTf.localScale = Vector3.zero;

        gameObject.SetActive(true);

        if(withAnim)
        {
            openSeq = DOTween.Sequence()
                .Append(_rootTf.DOScale(1, _animDuration));
            
        }
        else
        {
            _rootTf.localScale = Vector3.one;
        }
    }

    public virtual void Close(bool withAnim = true)
    {
        if (closeSeq != null) closeSeq.Kill();

        _rootTf.localScale = Vector3.one;

        gameObject.SetActive(false);

        if (withAnim)
        {
            closeSeq = DOTween.Sequence()
                .Append(_rootTf.DOScale(0, _animDuration));

        }
        else
        {
            _rootTf.localScale = Vector3.zero;
        }
    }
}
