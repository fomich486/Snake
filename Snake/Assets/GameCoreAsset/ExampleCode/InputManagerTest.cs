using InputManagerModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManagerTest : MonoBehaviour
{
    public Color UsualColor = Color.white;
    public Color ClickedColor = Color.red;

    public Text XValue;
    public Text YValue;
    //button1
    public Image Img1;
    private Coroutine anim1;
    //button2
    public Image Img2;
    private Coroutine anim2;
    //button3
    public Image Img3;
    private Coroutine anim3;
    //swipe left
    public Image Img4;
    private Coroutine anim4;
    //swipe right
    public Image Img5;
    private Coroutine anim5;
    //swipe top
    public Image Img6;
    private Coroutine anim6;

    private void Start()
    {
        //register listeners
        InputManager.Instance.Button1.AddListener(Button1Start);
        InputManager.Instance.Button1Up.AddListener(Button1End);
        InputManager.Instance.Button2.AddListener(Button2Start);
        InputManager.Instance.Button2Up.AddListener(Button2End);
        InputManager.Instance.Button3.AddListener(Button3);
        InputManager.Instance.SwipeLeft.AddListener(LeftSwipe);
        InputManager.Instance.SwipeRight.AddListener(RightSwipe);
        InputManager.Instance.SwipeTop.AddListener(TopSwipe);
        InputManager.Instance.MovingAxisX.AddListener(ChangeX);
        InputManager.Instance.MovingAxisY.AddListener(ChangeY);
    }

    //perform them
    void ChangeX(float _x)
    {
        XValue.text = (1f * (int)(_x * 100f) / 100f).ToString();
    }

    void ChangeY(float _y)
    {
        YValue.text = (1f * (int)(_y * 100f) / 100f).ToString();
    }

    void Button1Start()
    {
        if (anim1 != null)
        {
            StopCoroutine(anim1);
            anim1 = null;
        }
        Img1.color = ClickedColor;
    }

    void Button1End()
    {
        StartCoroutine(AnimateFade(Img1));
    }

    void Button2Start()
    {
        if (anim2 != null)
        {
            StopCoroutine(anim2);
            anim2 = null;
        }
        Img2.color = ClickedColor;
    }

    void Button2End()
    {
        StartCoroutine(AnimateFade(Img2));
    }

    void Button3()
    {
        if (anim3 != null)
        {
            StopCoroutine(anim3);
            anim3 = null;
        }
        Img3.color = ClickedColor;
        StartCoroutine(AnimateFade(Img3));
    }

    void LeftSwipe()
    {
        if (anim4 != null)
        {
            StopCoroutine(anim4);
            anim4 = null;
        }
        Img4.color = ClickedColor;
        StartCoroutine(AnimateFade(Img4));
    }

    void RightSwipe()
    {
        if (anim5 != null)
        {
            StopCoroutine(anim5);
            anim5 = null;
        }
        Img5.color = ClickedColor;
        StartCoroutine(AnimateFade(Img5));
    }

    void TopSwipe()
    {
        if (anim6 != null)
        {
            StopCoroutine(anim6);
            anim6 = null;
        }
        Img6.color = ClickedColor;
        StartCoroutine(AnimateFade(Img6));
    }

    IEnumerator AnimateFade(Image _img)
    {
        float _lerpCoef = 0f;
        float _lerpSpeed = 1f / 0.5f;
        while (_lerpCoef < 1)
        {
            _img.color = Color.Lerp(ClickedColor, UsualColor, _lerpCoef);
            _lerpCoef += TimeScaleManager.Instance.GetDelta();
            yield return new WaitForEndOfFrame();
        }
    }
}
