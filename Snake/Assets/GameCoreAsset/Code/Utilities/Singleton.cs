using UnityEngine;
using System.Collections;

/// <summary>
/// Singleton base class. Do not use it directly, inherit concrete singletons with CRTP: https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // Analysis disable once StaticFieldInGenericType
    protected static T instance;
    public static T Instance { get { return instance; } }
    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = (T)this;
    }
}