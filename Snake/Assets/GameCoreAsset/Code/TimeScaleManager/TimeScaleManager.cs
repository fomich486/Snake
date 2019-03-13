using UnityEngine;

/// <summary>
/// Class for virtual time in game
/// </summary>
public class TimeScaleManager : Singleton<TimeScaleManager>
{
    /// <summary>
    /// Virtual multiplier (similar as Time.timeScale)
    /// </summary>
    [SerializeField]
    private float virtualMultiplier = 1f;
    /// <summary>
    /// Virtual time (similar as Time.time)
    /// </summary>
    [SerializeField]
    private float virtualTime = 0f;
    /// <summary>
    /// Vitual deltaTime (similar as Time.deltaTime)
    /// </summary>
    [SerializeField]
    private float virtualDeltaTime = 0f;

    /// <summary>
    /// Virtual fixed delta time (similar as Time.fixedDeltaTime)
    /// </summary>
    private float virtualFixedDeltaTime = 0f;

	// Update is called once per frame
	void Update ()
    {
        virtualFixedDeltaTime = Time.fixedDeltaTime * virtualMultiplier;
        virtualDeltaTime = Time.deltaTime * virtualMultiplier;
        virtualTime += virtualDeltaTime;	
	}

    /// <summary>
    /// Call it to get virtual delta time (similar as Time.deltaTime)
    /// </summary>
    /// <returns></returns>
    public float GetDelta()
    {
        return virtualDeltaTime;
    }

    /// <summary>
    /// Call it to get virtual fixed delta time (similar as Time.fixedDeltaime)
    /// </summary>
    /// <returns></returns>
    public float GetFixedDelta()
    {
        return virtualFixedDeltaTime;
    }

    /// <summary>
    /// Call it to get virtual time (similar as Time.time)
    /// </summary>
    /// <returns></returns>
    public float GetTime()
    {
        return virtualTime;
    }

    /// <summary>
    /// Call this function to set time scale (similar as Time.timeScale)
    /// </summary>
    /// <param name="_value"></param>
    public void SetScale(float _value)
    {
        virtualMultiplier = _value;
    }

    /// <summary>
    /// Call this function to get current time scale (similar as Time.timeScale)
    /// </summary>
    /// <returns></returns>
    public float GetScale()
    {
        return virtualMultiplier;
    }
}
