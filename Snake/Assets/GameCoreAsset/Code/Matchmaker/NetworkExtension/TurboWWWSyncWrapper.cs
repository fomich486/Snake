using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TurboWWWSyncWrapper
{
    private static TurboWWWSyncWrapper instance;

    private Attributes attributes;
    private bool isLog = true;

    private TurboWWWSyncWrapper()
    {
        attributes = new Attributes();
        attributes.Retries = 3;
        attributes.RetriesDelay = 1f;
        attributes.Headers = new Dictionary<string, string>();
        attributes.Headers.Add("Content-Type", "application/json");
    }

    public static TurboWWWSyncWrapper Instance
    {
        get
        {
            if(instance==null)
            {
                instance = new TurboWWWSyncWrapper();
            }
            return instance;
        }
    }


   

    public void Get(string _url, Action<string> _onAnswer, Action<string> _onError = null)
    {
        var _www = new TurboWWW();
        TurboWWW _tWWW = new TurboWWW();
        _tWWW.UrlName = _url;
        _tWWW.ReqType = RequestType.GET;
        _tWWW.Dict = attributes.Headers;
        _tWWW.Retries = attributes.Retries;
        _tWWW.RetriesDelay = attributes.RetriesDelay;
        if (_onError != null) _tWWW.OnErrorEvent.AddListener((_error)=> { _onError(_error); if (isLog) Log(_error); });
        _tWWW.OnResultEvent.AddListener((_answer)=> { _onAnswer(_answer); if (isLog) Log(_answer); });
        _tWWW.Execute();
    }

    public void Post(string _url, string _jsData, Action<string> _onAnswer, Action<string> _onError = null)
    {
        var _www = new TurboWWW();
        TurboWWW _tWWW = new TurboWWW();
        _tWWW.UrlName = _url;
        _tWWW.Data = _jsData;
        _tWWW.ReqType = RequestType.POST;
        _tWWW.Dict = attributes.Headers;
        _tWWW.Retries = attributes.Retries;
        _tWWW.RetriesDelay = attributes.RetriesDelay;
        if (_onError != null) _tWWW.OnErrorEvent.AddListener((_error) => { _onError(_error); if (isLog) Log(_error); });
        _tWWW.OnResultEvent.AddListener((_answer) => { _onAnswer(_answer); if (isLog) Log(_answer); });
        _tWWW.Execute();
    }

    public void Put(string _url, string _jsData, Action<string> _onAnswer, Action<string> _onError = null)
    {
        var _www = new TurboWWW();
        TurboWWW _tWWW = new TurboWWW();
        _tWWW.UrlName = _url;
        _tWWW.Data = _jsData;
        _tWWW.ReqType = RequestType.PUT;
        _tWWW.Dict = attributes.Headers;
        _tWWW.Retries = attributes.Retries;
        _tWWW.RetriesDelay = attributes.RetriesDelay;
        if (_onError != null) _tWWW.OnErrorEvent.AddListener((_error) => { _onError(_error); if (isLog) Log(_error); });
        _tWWW.OnResultEvent.AddListener((_answer) => { _onAnswer(_answer); if (isLog) Log(_answer); });
        _tWWW.Execute();
    }

    public void Delete(string _url, string _jsData, Action<string> _onAnswer, Action<string> _onError = null)
    {
        var _www = new TurboWWW();
        TurboWWW _tWWW = new TurboWWW();
        _tWWW.UrlName = _url;
        _tWWW.ReqType = RequestType.DELETE;
        _tWWW.Dict = attributes.Headers;
        _tWWW.Retries = attributes.Retries;
        _tWWW.RetriesDelay = attributes.RetriesDelay;
        if(_onError!=null)_tWWW.OnErrorEvent.AddListener((_error) => { _onError(_error); if (isLog) Log(_error); });
        _tWWW.OnResultEvent.AddListener((_answer) => { _onAnswer(_answer); if (isLog) Log(_answer); });
        _tWWW.Execute();
    }

    public void GetImage(string _url, Action<Texture2D> _onAnswer, Action<string> _onError = null)
    {
        TurboWWW _tWWW = new TurboWWW();
        _tWWW.UrlName = _url;
        _tWWW.ReqType = RequestType.GET_TEXTURE;
        _tWWW.Retries = attributes.Retries;
        _tWWW.RetriesDelay = attributes.RetriesDelay;
        if (_onError != null) _tWWW.OnErrorEvent.AddListener((_error) => { _onError(_error); if (isLog) Log(_error); });
        _tWWW.OnTextureEvent.AddListener((_answer) => { _onAnswer(_answer); if (isLog) Log("texture gained"); });
        _tWWW.Execute();
    }

    private void Log(string s)
    {
        //ToDo NetworkLogger
        Debug.Log("<color=blue>NetworkMessage: <=(Receive): "+s+"</color>");
    }


    private struct Attributes
    {
        public int Retries;
        public float RetriesDelay;
        public Dictionary<string, string> Headers;
        //etc
    }
}
