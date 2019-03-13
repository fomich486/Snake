using Analytics; 
using System.Collections.Generic;
using UnityEngine; 

/// <summary>
/// Class for performing analytics
/// </summary>
public class AnalyticsManager : Singleton<AnalyticsManager>
{ 
    //keys for analytics
    /// <summary>
    /// iOS analytics key
    /// </summary>
    public string IOSKey = "TVYVBXXC64NJC99XPSFD";
    /// <summary>
    /// Android analytics key
    /// </summary>
    public string AndroidKey = "WHC8TC5WWSJ6TN7H46BZ";
     
    /// <summary>
    /// Call it in the start of your app
    /// </summary>
    private void Awake()
    {
        base.Awake(); 
    }  

    private void Start()
    {
        //register flurry session
        if (Flurry.Instance != null)
        {
            Flurry.Instance.StartSession(IOSKey, AndroidKey);
            string _userId = SystemInfo.deviceUniqueIdentifier;
            if (_userId == SystemInfo.unsupportedIdentifier)
            {
                _userId = SystemInfo.deviceName + SystemInfo.deviceModel + Random.Range(0, 99999999999);
            }
            Flurry.Instance.LogUserID(_userId); 
        }
    } 

    /// <summary>
    /// Call it to make event
    /// </summary>
    /// <param name="_name"></param>
    public void MakeEvent(string _name)
    {
        Log.Write("Making event:" + _name, LogColors.Grey);
        Flurry.Instance.LogEvent(_name);
    }

    /// <summary>
    /// Call it to make event with parameters
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_key"></param>
    /// <param name="_keyValue"></param>
    public void MakeEvent(string _name, string _key, string _keyValue)
    {
        Log.Write("Making event:" + _name + " =!= " +_key + " ==!!== " + _keyValue, LogColors.Grey);

        Dictionary<string, string> _dict = new Dictionary<string, string>();
        _dict.Add(_key, _keyValue);

        Flurry.Instance.LogEvent(_name, _dict);
    } 
}
