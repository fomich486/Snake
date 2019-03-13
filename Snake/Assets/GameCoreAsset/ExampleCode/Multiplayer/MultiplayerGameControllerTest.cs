using NetworkExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class MultiplayerGameControllerTest : NetworkBehaviour
{

    public GameObject SpawnablePrefab;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if(isServer)
            {
                NetworkServer.Spawn(Instantiate(SpawnablePrefab));
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("OnStartClient");
        ClientScene.RegisterPrefab(SpawnablePrefab);
    }

    private void OnDisable()
    {
        Debug.Log("Disable");
    }
}
