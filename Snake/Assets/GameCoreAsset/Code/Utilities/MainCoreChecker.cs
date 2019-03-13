using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that checkf for main core and destroys current if another has been found
/// </summary>
public class MainCoreChecker : MonoBehaviour
{
    private void Awake()
    {
        MainCoreChecker _core = GameObject.FindObjectOfType<MainCoreChecker>();
        if (_core != null && _core != this)
            Destroy(this.gameObject); 
    }
}
