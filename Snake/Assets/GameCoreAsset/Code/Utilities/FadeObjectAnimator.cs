using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for animation of Image from-to some color with different parameters
/// </summary>
public class FadeObjectAnimator : MonoBehaviour
{
    /// <summary>
    /// List of images to fade in-out
    /// </summary>
    public Image[] ImagesToAnimate;
    /// <summary>
    /// Start color
    /// </summary>
    public Color From = Color.white;
    /// <summary>
    /// End color
    /// </summary>
    public Color To = Color.white;
    /// <summary>
    /// Time for From->To
    /// </summary>
    public float ToPassTime = 1f;
    /// <summary>
    /// Time for To->From
    /// </summary>
    public float BackPassTime = 1f;
    /// <summary>
    /// Flag if animation will be like: From->To->From->To...
    /// </summary>
    public bool PingPong = true;
    /// <summary>
    /// Flag if this animation will depend on global time or TimeScaleManager
    /// </summary>
    public bool GlobalTime = false;

    /// <summary>
    /// Called in the start
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine(AnimateFade());
    }

    /// <summary>
    /// Coroutine of fade animation
    /// </summary>
    /// <returns></returns>
    IEnumerator AnimateFade()
    {
        yield return new WaitForEndOfFrame();

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

                SetImagesTo(Color.Lerp(From, To, _lerpCoef));
                yield return new WaitForEndOfFrame();
            }

            if (PingPong)
            {
                _lerpSpeed = 1f / BackPassTime;
                while (_lerpCoef > 0)
                {
                    if (GlobalTime)
                        _lerpCoef -= Time.deltaTime * _lerpSpeed;
                    else
                        _lerpCoef -= TimeScaleManager.Instance.GetDelta() * _lerpSpeed;

                    SetImagesTo(Color.Lerp(From, To, _lerpCoef));
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }

    /// <summary>
    /// Call it to set some color to images
    /// </summary>
    /// <param name="_c"></param>
    void SetImagesTo(Color _c)
    {
        foreach (Image _i in ImagesToAnimate)
        {
            _i.color = _c;
        }
    }
}
