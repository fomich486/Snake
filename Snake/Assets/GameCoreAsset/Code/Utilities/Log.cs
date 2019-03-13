using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : Singleton<Log>
{
    /// <summary>
    /// Flag if this build debug (if not - logs will not appear)
    /// </summary>
    public static bool IsDebug = true; 

    /// <summary>
    /// Awake function called in the start. we decide here do we need logs
    /// </summary>
    private void Awake()
    {
        IsDebug = Debug.isDebugBuild;
        Write("Set up " + IsDebug, LogColors.Black, 30);

        base.Awake();
    }

    /// <summary>
    /// Call it to write some log with set text size
    /// </summary>
    /// <param name="_message"></param>
    /// <param name="_size"></param>
    public static void Write(string _message, int _size)
    {
        Write(_message, LogColors.Black, _size);
    }

    /// <summary>
    /// Call it to write some log with color and size of text
    /// </summary>
    /// <param name="_message"></param>
    /// <param name="_color"></param>
    /// <param name="_size"></param>
    public static void Write(string _message, LogColors _color, int _size = 12)
    {
        if (!IsDebug)
            return;

        string _totalDebug = "";
        switch (_color)
        {
            case LogColors.Black:
                _totalDebug = "<color=black>";
                break;
            case LogColors.Grey:
                _totalDebug = "<color=grey>";
                break;
            case LogColors.White:
                _totalDebug = "<color=white>";
                break;
            case LogColors.Blue:
                _totalDebug = "<color=blue>";
                break;
            case LogColors.Red:
                _totalDebug = "<color=red>";
                break;
            case LogColors.Yellow:
                _totalDebug = "<color=yellow>";
                break;
            case LogColors.Green:
                _totalDebug = "<color=green>";
                break;
            default:
                _totalDebug = "<color=black>";
                break;
        }
        _totalDebug += "<size=" + _size + ">";
        _totalDebug += _message;
        _totalDebug += "</size>";
        _totalDebug += "</color>";  
        Debug.Log(_totalDebug);
    }     

    /// <summary>
    /// Call it to write usual log
    /// </summary>
    /// <param name="_message"></param>
    public static void Write(string _message)
    {
        if (!IsDebug)
            return;
         
        Debug.Log(_message);
    }
}

public enum LogColors
{
    Black,
    Grey,
    White,
    Blue,
    Red,
    Yellow,
    Green,
}
