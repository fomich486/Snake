using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class for some http transactions
/// </summary>
public class TurboWWW
{
    #region VARIABLES
    /// <summary>
    /// Name of requested URL
    /// </summary>
    public string UrlName = "";
    /// <summary>
    /// Type of request
    /// </summary>
    public RequestType ReqType = RequestType.NONE;
    /// <summary>
    /// Data in string type
    /// </summary>
    public string Data = "";
    /// <summary>
    /// Data in byte type
    /// </summary>
    public byte[] ByteData = null;
    /// <summary>
    /// Count of retries
    /// </summary>
    public int Retries = 0;
    /// <summary>
    /// Delay before initialization
    /// </summary>
    public float Delay = 0f;
    /// <summary>
    /// Delay between retries
    /// </summary>
    public float RetriesDelay = 4f;
    /// <summary>
    /// Dictionary of some additional headers
    /// </summary>
    public Dictionary<string, string> Dict = new Dictionary<string, string>();
    #endregion

    #region CALLBACKS
    //CALLED IN THE START OF REQUEST
    private UnityStringEvent onStartEvent;
    /// <summary>
    /// Called when start event is catched
    /// </summary>
    public UnityStringEvent OnStartEvent
    {
        get
        {
            if (onStartEvent == null)
                onStartEvent = new UnityStringEvent();

            return onStartEvent;
        }
        set
        {
            onStartEvent = value;
        }
    }

    //CALLED TO CATCH RESULT OF REQUEST
    private UnityStringEvent onResultEvent;
    /// <summary>
    /// Called when result is catched
    /// </summary>
    public UnityStringEvent OnResultEvent
    {
        get
        {
            if (onResultEvent == null)
                onResultEvent = new UnityStringEvent();

            return onResultEvent;
        }
        set
        {
            onResultEvent = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private UnityStringStringEvent onResultHeadersEvent;
    /// <summary>
    /// Called when headers are catched
    /// </summary>
    public UnityStringStringEvent OnResultHeadersEvent
    {
        get
        {
            if (onResultHeadersEvent == null)
                onResultHeadersEvent = new UnityStringStringEvent();

            return onResultHeadersEvent;
        }
        set
        {
            onResultHeadersEvent = value;
        }
    }
     
    //CALLED TO CATCH ERRORS 
    private UnityStringEvent onErrorEvent;
    /// <summary>
    /// Called when some error is catched
    /// </summary>
    public UnityStringEvent OnErrorEvent
    {
        get
        {
            if (onErrorEvent == null)
                onErrorEvent = new UnityStringEvent();

            return onErrorEvent;
        }
        set
        {
            onErrorEvent = value;
        }
    }

    //CALLED TO CATCH TEXTURE CALLBACKS
    private UnityTextureEvent onTextureEvent;
    /// <summary>
    /// Called when texture is loaded
    /// </summary>
    public UnityTextureEvent OnTextureEvent
    {
        get
        {
            if (onTextureEvent == null)
                onTextureEvent = new UnityTextureEvent();

            return onTextureEvent;
        }
        set
        {
            onTextureEvent = value;
        }
    }
    #endregion

    #region INIT_METHODS
    /// <summary>
    /// Call it to init request
    /// </summary>
    /// <param name="_url">URL of requesst</param>
    /// <param name="_type">Type of request</param>
    /// <param name="_data">Data in string of request</param>
    /// <param name="_retries">Count of retires</param>
    /// <param name="_delay">Time between retries</param>
    public void Init(string _url, RequestType _type, string _data = "", int _retries = 0, float _delay = 0f)
    {
        if (string.IsNullOrEmpty(_url))
        {
            OnErrorEvent.Invoke("There is no url, or it's empty to process request");
            Log.Write("There is no url, or it's empty to process request",LogColors.Yellow);
            return;
        }
        UrlName = _url;
        ReqType = _type;
        Data = _data;
        Retries = _retries;
        RetriesDelay = _delay;
    }

    /// <summary>
    /// Call it to init request
    /// </summary>
    /// <param name="_url">URL of requesst</param>
    /// <param name="_type">Type of request</param>
    /// <param name="_byteData">Data in bytes of request</param>
    /// <param name="_data">Data in string of request</param>
    /// <param name="_retries">Count of retires</param>
    /// <param name="_delay">Time between retries</param>
    public void Init(string _url, RequestType _type, byte[] _byteData, string _data, int _retries = 0, float _delay = 0f)
    {
        if (string.IsNullOrEmpty(_url))
        {
            OnErrorEvent.Invoke("There is no url, or it's empty to process request");
            Log.Write("There is no url, or it's empty to process request",LogColors.Yellow);
            return;
        }
        UrlName = _url;
        ReqType = _type;
        Data = _data;
        ByteData = _byteData;
        Retries = _retries;
        RetriesDelay = _delay;
    }

    /// <summary>
    /// Call it to init request
    /// </summary>
    /// <param name="_url">URL of requesst</param>
    /// <param name="_type">Type of request</param>
    /// <param name="_byteData">Data in bytes of request</param> 
    /// <param name="_retries">Count of retires</param>
    /// <param name="_delay">Time between retries</param>
    public void Init(string _url, RequestType _type, byte[] _byteData, int _retries = 0, float _delay = 0f)
    {
        if (string.IsNullOrEmpty(_url))
        {
            OnErrorEvent.Invoke("There is no url, or it's empty to process request");
            Log.Write("There is no url, or it's empty to process request",LogColors.Yellow);
            return;
        }
        UrlName = _url;
        ReqType = _type; 
        ByteData = _byteData;
        Retries = _retries;
        RetriesDelay = _delay;
    }
    #endregion

    #region CALLING_METHODS

    /// <summary>
    /// Call it to execute request
    /// </summary>
    public void Execute()
    {
        if (string.IsNullOrEmpty(UrlName))
        {
            OnErrorEvent.Invoke("There is no url, or it's empty to process request");
            Log.Write("There is no url, or it's empty to process request",LogColors.Yellow);
            return;
        }

        TurboWWWManager.Instance.StartCoroutine(ExecuteRequest());
    }

    /// <summary>
    /// Call it to execute request in coroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator ExecuteRequest()
    {
        // Waiting for request  delay
        yield return new WaitForSeconds(Delay);

        Dictionary<string, string> _dict = new Dictionary<string, string>();
        //
        //  FILLING REQUEST TYPE
        //
        UnityWebRequest _newRequest = null;
        switch (ReqType)
        {
            case RequestType.GET:
                _newRequest = UnityWebRequest.Get(UrlName);
                _dict.Add("X-HTTP-Method-Override", "GET");
                break;

            case RequestType.POST:
                _newRequest = UnityWebRequest.Post(UrlName, Data);
                _dict.Add("X-HTTP-Method-Override", "POST");
                break;

            case RequestType.PUT:
                _newRequest = UnityWebRequest.Put(UrlName, ByteData);
                _dict.Add("X-HTTP-Method-Override", "PUT");
                break;

            case RequestType.DELETE:
                _newRequest = UnityWebRequest.Delete(UrlName);
                _dict.Add("X-HTTP-Method-Override", "DELETE");
                break;

            case RequestType.GET_TEXTURE:
                _newRequest = UnityWebRequestTexture.GetTexture(UrlName);
                break;
            case RequestType.NONE:
                Log.Write("NONE Request type incomed",LogColors.Yellow);
                //nothing to do
                //it's not set
                break;
        }
        //
        //  FILLING DATA
        //
        if (!string.IsNullOrEmpty(Data))
        {
            //TO DO - REMOVE COMMENT

            _dict.Add("X-HTTP-DATA", Data);
        }
        // FILLING DATA
        foreach (KeyValuePair<string, string> _pair in Dict)
        {
            //TO DO - REMOVE COMMENT

            _dict.Add(_pair.Key, _pair.Value);
        }

        
        //waiting for response
        bool _expired = false;
        if (Retries > 0 || RetriesDelay > 0)
        {
            _newRequest.Send();
            float _currentWaitTime = RetriesDelay;
            int _currentRetriesLeft = Retries;
            while (!_newRequest.isDone && !_expired && string.IsNullOrEmpty(_newRequest.error))
            {
                yield return new WaitForEndOfFrame();
                _currentWaitTime -= Time.deltaTime;
                if (_currentWaitTime < 0)
                {
                    _currentWaitTime = RetriesDelay;
                    _currentRetriesLeft--;
                    if (_currentRetriesLeft <= 0)
                    {
                        _expired = true;
                        _newRequest.Dispose();
                    }
                }
            }
        }
        else
        {
            yield return _newRequest.Send();
        }

        //if we got some error
        if (!string.IsNullOrEmpty(_newRequest.error))
        {
            Log.Write(_newRequest.error + " Request URL: " + UrlName,LogColors.Yellow);
            OnErrorEvent.Invoke(_newRequest.error + " Request URL: " + UrlName);
            yield break;
        }
        //if we haven't time
        else if (_expired)
        {
            Log.Write("Request is out ot retires. Request URL:" + UrlName,LogColors.Yellow);
            OnErrorEvent.Invoke("Request is out ot retires. Request URL:" + UrlName);
            yield break;
        }
        else if (_newRequest.isDone)
        {
            if (!string.IsNullOrEmpty(_newRequest.downloadHandler.text))
                OnResultEvent.Invoke(_newRequest.downloadHandler.text);

            if (_newRequest.GetResponseHeaders().Count > 0)
                OnResultHeadersEvent.Invoke(_newRequest.GetResponseHeaders());

            if (_newRequest.downloadHandler!=null && _newRequest.downloadHandler as DownloadHandlerTexture != null && ((DownloadHandlerTexture)_newRequest.downloadHandler).texture != null)
                OnTextureEvent.Invoke(((DownloadHandlerTexture)_newRequest.downloadHandler).texture);
        }
        else
        {
            Log.Write("Havent done request. Request URL:" + UrlName,LogColors.Yellow);
            OnErrorEvent.Invoke("Havent done request. Request URL:" + UrlName);
            yield break;
        }
    }
    #endregion
}
