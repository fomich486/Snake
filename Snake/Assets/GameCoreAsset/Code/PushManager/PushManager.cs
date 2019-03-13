using Assets.SimpleAndroidNotifications;
using DataSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

public class PushManager : Singleton<PushManager>
{ 
    /// <summary>
    /// ID of OneSignal SDK
    /// </summary>
    public string NotificationID = "5e9a1d77-046a-40cd-a2c3-cd918d78fd60";
    /// <summary>
    /// ID of player 
    /// </summary>
    private string playerID = "";
    /// <summary>
    /// State when we inited OneSingal
    /// </summary>
    public bool Inited = false;
    /// <summary>
    /// Flag if nnotifications registered from balance file
    /// </summary>
    private bool readNotifications = false;
    /// <summary>
    /// Dicitonary of registered notifications
    /// </summary>
    private Dictionary<string, int> NotificationsDict = new Dictionary<string, int>();

    /// <summary>
    /// Last time of trying to init pushes
    /// </summary>
    private float lastTryTime = 0f;
    /// <summary>
    /// Delay between tryins initing pushes
    /// </summary>
    public float TryDelay = 10f;  

    private void Start()
    {
        //READING INFO FROM BALANCE AND REGISTRATIONING PUSHES

        //if (DataManager.Instance.balanceData.RetentionNotification != null && DataManager.Instance.balanceData.RetentionNotification.Count > 0)
        //{
        //    readNotifications = true;
        //    Days = DataManager.Instance.balanceData.RetentionNotification;

        //    DateTime _now = DateTime.Today;
        //    foreach (SerializedDictDate _d in Days)
        //        AddNotification(_d.Text, ((24 + 16) * 60 * 60));
        //} 

#if UNITY_IOS
        NotificationServices.RegisterForNotifications(
            NotificationType.Alert |
            NotificationType.Badge |
            NotificationType.Sound);
#endif
    }

    /// <summary>
    /// Inits push notification managers (for now, it's OneSignal)
    /// </summary>
    public void InitPushes()
    {  
        lastTryTime = Time.time;

        Log.Write("BEFORE INIT ONESINGAL", LogColors.Yellow, 18); 

        OneSignal.StartInit(NotificationID).HandleNotificationOpened(HandleNotificationOpened).EndInit();

        playerID = OneSignal.GetPermissionSubscriptionState().subscriptionStatus.userId;

        if (!string.IsNullOrEmpty(playerID))
        {
            Log.Write("Got player id:" + playerID, LogColors.White);

            OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;

            Log.Write("AFTER INIT ONESINGAL", LogColors.Yellow, 18);

            Inited = true;
        }
    }

    /// <summary>
    /// Call it to register notification to dictionary. 
    /// </summary>
    /// <param name="_text">Text of notification</param>
    /// <param name="_time">Seconds of delay</param>
    public void AddNotification(string _text, int _time)
    {
        if (NotificationsDict.ContainsValue(_time))
            return;

        if (NotificationsDict.ContainsKey(_text))
            return;

        Log.Write("Adding notifications: " + _text + " date:" + _time, LogColors.Blue);
        NotificationsDict.Add(_text, _time);
    }

    /// <summary>
    /// Call it to remove some notification from dictionary
    /// </summary>
    /// <param name="_text"></param>
    public void RemoveNotification(string _text)
    {
        if (NotificationsDict.ContainsKey(_text))
        {
            NotificationsDict.Remove(_text);
            Log.Write("Removing notifications: " + _text, LogColors.Blue);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        // clearing notifications
        if (focus)
        {
            Log.Write("ON FOCUSED CLEARING ALL PUSHES!");
#if UNITY_ANDROID
            NotificationManager.CancelAll();
            Log.Write("ON FOCUSED CLEARING ALL PUSHES! UNITY_ANDROID");
#elif UNITY_IOS 
            NotificationServices.ClearLocalNotifications();
            NotificationServices.CancelAllLocalNotifications();
            Log.Write("ON FOCUSED CLEARING ALL PUSHES! UNITY_IOS");
#endif
        }
        // Sending notifications
        else
        {
            Log.Write("ON DEFOCUSED SENDING ALL PUSHES!");
            foreach (KeyValuePair<string, int> _p in NotificationsDict)
            {
                MakeNotification(_p.Key, _p.Value);
            } 
        }
    }

    /// <summary>
    /// Call it to send local notification
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_afterSeconds"></param>
    public void MakeNotification(string _text, int _afterSeconds)
    {
#if UNITY_ANDROID
        NotificationManager.SendWithAppIcon(TimeSpan.FromSeconds(_afterSeconds), "HedgeHop", _text, new Color(0, 0.6f, 1), NotificationIcon.Message);
#elif UNITY_IOS
        var notif = new LocalNotification();
        notif.fireDate = DateTime.Now.AddSeconds(_afterSeconds);
        notif.alertBody = _text; 
        NotificationServices.ScheduleLocalNotification(notif);
#endif
    }
      
    /// <summary>
    /// Handler of opened notification (by OneSignal)
    /// </summary>
    /// <param name="result"></param>
    private void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        Log.Write("OPENED NOTIFCATION!", LogColors.Blue, 18);
        Log.Write("Handled opened notification:" + result.notification.ToString(), LogColors.Blue, 18);
    }

    private void Update()
    {  
        // trying to init pushes
        if (!Inited && lastTryTime + TryDelay <= Time.time)
        {
            InitPushes();
        } 
    }
}

[Serializable]
public class SerializedDictDate
{
    public string Text;
    public int DaysAfter;
}
