using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityIntEvent : UnityEvent<int>
{

}

[System.Serializable]
public class UnityFloatEvent : UnityEvent<float>
{

}

[System.Serializable]
public class UnityStringEvent : UnityEvent<string>
{

}

[System.Serializable]
public class UnityStringStringEvent : UnityEvent<Dictionary<string, string>>
{

}

[System.Serializable]
public class UnityTextureEvent : UnityEvent<Texture2D>
{

}
