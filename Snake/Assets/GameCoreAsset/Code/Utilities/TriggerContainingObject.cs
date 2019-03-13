using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component that contains list of object that entered to trigger, and that moved out from it
/// </summary>
public class TriggerContainingObject : MonoBehaviour
{ 
    /// <summary>
    /// List of objects
    /// </summary>
    public List<Transform> EnteredObjects = new List<Transform>();
	
    /// <summary>
    /// Call it to get all object in trigger
    /// </summary>
    /// <returns></returns>
    public Transform[] GetAllObjects()
    {
        return EnteredObjects.ToArray();
    }

    /// <summary>
    /// When somebody enters trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (!EnteredObjects.Contains(other.gameObject.transform))
        {
            EnteredObjects.Add(other.gameObject.transform);
        }
    }

    /// <summary>
    /// When somebody exits out of trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (EnteredObjects.Contains(other.gameObject.transform))
        {
            EnteredObjects.Remove(other.gameObject.transform);
        }
    }  
}
