using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurboWWWTest : MonoBehaviour
{
    public SpriteRenderer Spr;

    private void Start()
    {
        TurboWWW _tWWW = new TurboWWW();
        _tWWW.UrlName = "https://www.ushki-ruchki.ru/uploads/images/h/e/l/hello.jpg";
        _tWWW.ReqType = RequestType.GET_TEXTURE;
        _tWWW.Retries = 3;
        _tWWW.RetriesDelay = 1f;
        _tWWW.OnErrorEvent.AddListener(GotError);
        _tWWW.OnTextureEvent.AddListener(GotTexture);

        _tWWW.Execute();
    }

    void GotTexture(Texture2D _t)
    {
        Sprite _loadedSprite = Sprite.Create(_t, new Rect(0.0f, 0.0f, _t.width, _t.height), new Vector2(0.5f, 0.5f), 100.0f);
        Spr.sprite = _loadedSprite; 
    }

    void GotError(string _err)
    {
        Log.Write("we got error:" + _err, LogColors.Red, 18);
    }
}
