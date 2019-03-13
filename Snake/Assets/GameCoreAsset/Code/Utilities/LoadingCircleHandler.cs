using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class for animation of loading circle
/// </summary>
public class LoadingCircleHandler : MonoBehaviour
{
    /// <summary>
    /// length of animation
    /// </summary>
    public float AnimationTime = 1f;
    /// <summary>
    /// Animation image
    /// </summary>
    public Image LoadingCircle;

    private void OnEnable()
    {
        StartCoroutine(AnimateCircle());
    }

    IEnumerator AnimateCircle()
    {
        float _lerpCoef = 0f;
        float _lerpSpeed = 1f / AnimationTime;

        while (true)
        {
            //fill
            LoadingCircle.fillClockwise = true;
            _lerpCoef = 0f;
            while (_lerpCoef < 1)
            {
                _lerpCoef += Time.deltaTime * _lerpSpeed;
                LoadingCircle.fillAmount = _lerpCoef;
                yield return new WaitForEndOfFrame();
            }
            //clear
            LoadingCircle.fillClockwise = false;  
            while (_lerpCoef > 0)
            {
                _lerpCoef -= Time.deltaTime * _lerpSpeed;
                LoadingCircle.fillAmount = _lerpCoef;
                yield return new WaitForEndOfFrame();
            } 
        }
    }
}
