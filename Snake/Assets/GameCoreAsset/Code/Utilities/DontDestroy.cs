using UnityEngine;

/// <summary>
/// Put this component to some GameObject and it will not be destroyed
/// </summary>
public class DontDestroy : MonoBehaviour
{ 
	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);	
	} 
}
