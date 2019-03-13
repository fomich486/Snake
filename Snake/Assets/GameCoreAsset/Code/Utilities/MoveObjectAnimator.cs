using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for animation position of object
/// </summary>
public class MoveObjectAnimator : MonoBehaviour
{
    /// <summary>
    /// Cashed RectTransform of animationing object
    /// </summary>
    public RectTransform RTrans;
    /// <summary>
    /// Cashed Transform of obj
    /// </summary>
    public Transform Trans;

    /// <summary>
    /// Vector3 From
    /// </summary>
    public Vector3 PositionFrom;
    /// <summary>
    /// Vector3 to
    /// </summary>
    public Vector3 PositionTo;

    /// <summary>
    /// Flag if we use local space
    /// </summary>
    public bool LocalSpace;
    /// <summary>
    /// Flag if we use rect space (anchored position)
    /// </summary>
    public bool RectSpace;
    /// <summary>
    /// Flag if we use global time, or virtual
    /// </summary>
    public bool GlobalTime = true;
    /// <summary>
    /// Flag if animation have to be like (0->1, 1->0, 0->1) or (0->1, 0->1, 0->1)
    /// </summary>
    public bool PingPong = true;
    /// <summary>
    /// Time to pass To direction (0->1)
    /// </summary>
    public float ToPassTime = 1f;
    /// <summary>
    /// Time to pass Back direction (1->0)
    /// </summary>
    public float FromPassTime = 1f;

    /// <summary>
    /// Called to start animation
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(AnimateMoving());
    }

    /// <summary>
    /// Coroutine of move animation
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateMoving()
    {
        float _lerpCoef = 0f;
        float _lerpSpeed = 1f / ToPassTime;
        while (true)
        {
            _lerpCoef = 0f;
            _lerpSpeed = 1f / ToPassTime;

            while (_lerpCoef < 1)
            {
                if (GlobalTime)
                    _lerpCoef += Time.deltaTime * _lerpSpeed;
                else
                    _lerpCoef += TimeScaleManager.Instance.GetDelta() * _lerpSpeed;


                SetPosition(_lerpCoef);

                yield return new WaitForEndOfFrame();
            }

            if (PingPong)
            {
                _lerpSpeed = 1f / FromPassTime;
                while (_lerpCoef > 0)
                {
                    if (GlobalTime)
                        _lerpCoef -= Time.deltaTime * _lerpSpeed;
                    else
                        _lerpCoef -= TimeScaleManager.Instance.GetDelta() * _lerpSpeed;

                    SetPosition(_lerpCoef);

                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    /// <summary>
    /// Call it to set position depends on some lerp coef between FROM and TO position
    /// </summary>
    /// <param name="_lerpCoef"></param>
    void SetPosition(float _lerpCoef)
    { 
        if (RectSpace)
        {
            if (LocalSpace)
            {
                RTrans.anchoredPosition = Vector3.Lerp(PositionFrom, PositionTo, _lerpCoef);
            }
            else
            {
                RTrans.position = Vector3.Lerp(PositionFrom, PositionTo, _lerpCoef);
            }
        }
        else
        {
            if (LocalSpace)
            {
                Trans.localPosition = Vector3.Lerp(PositionFrom, PositionTo, _lerpCoef);
            }
            else
            {
                Trans.position = Vector3.Lerp(PositionFrom, PositionTo, _lerpCoef);
            }
        }
    }
}
