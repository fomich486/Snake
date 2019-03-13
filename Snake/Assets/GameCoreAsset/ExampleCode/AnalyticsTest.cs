using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsTest : MonoBehaviour
{
    private void Start()
    {
        AnalyticsManager.Instance.MakeEvent("Test event");

        AnalyticsManager.Instance.MakeEvent("Test event with params", "Test key", "Test value");
    }
}
