using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MatchMaker;
using UnityEngine;

using UnityEngine.Networking;

public class TouchLight : NetworkBehaviour
{
    private static List<Color> Colors = new List<Color> { Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };
    private static List<TouchLight> playerList;
    private Rigidbody rg;
    private Color color;
    private bool isColorGot;


    public GameObject Trail;
    // Use this for initialization
    void Start()
    {
        if (playerList == null) playerList = new List<TouchLight>();
        playerList.Add(this);
        gameObject.name = "Player " + playerList.Count;
        if (isLocalPlayer&&isServer)
        {
            Colors.Insert(1, new Color(246.0f / 255, 117.0f / 255, 2f / 255));
            Colors.Insert(4, new Color(207.0f / 255, 168.0f / 255, 139f / 255));
            Colors.Add(new Color(107.0f / 255, 46.0f / 255, 113f / 255));
            Colors.Add(new Color(0, 157f / 255, 1));
            Colors = Colors.OrderBy(a => Guid.NewGuid()).ToList();
            Debug.Log("Shake Colors");
        }
        rg = GetComponent<Rigidbody>();
        if (isLocalPlayer)
        {
            CmdSetName(gameObject.name);
            CmdSetColor();
            CmdSync();
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            rg.MovePosition(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5f)));
        }

    }

    [Command]
    private void CmdSetColor()
    {
        Debug.Log("CmdSetColor " + gameObject.name + " Colors[0] = " + Colors[0] + " Colors[1] = " + Colors[1]);
        if (playerList.Count >= Colors.Count)
        {
            Colors.Add(UnityEngine.Random.ColorHSV());
        }
        RpcSetColor(Colors[playerList.IndexOf(this)]);
        Debug.Log(gameObject.name + " index = " + playerList.IndexOf(this));
    }

    [Command]
    private void CmdSetName(string _name)
    {
        RpcOnChangeName(_name);
    }

    [ClientRpc]
    private void RpcOnChangeName(string _name)
    {
        gameObject.name = _name;
    }

    void OnGUI()
    {
        var style = new GUIStyle { fontSize = 50 };
        if (!isLocalPlayer)
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.Label(new Rect(pos.x, Camera.main.pixelHeight - pos.y, 500, 500), gameObject.name, style);
        }
        else
        {
            gameObject.name = GUI.TextArea(new Rect(10, 10, 500, 100), gameObject.name, style);
            if (GUI.Button(new Rect(520, 10, 100, 100), "Ok"))
            {
                CmdSetName(gameObject.name);
            }
        }
    }

    [ClientRpc]
    private void RpcSetColor(Color _color)
    {
        color = _color;
        var main = Trail.GetComponent<ParticleSystem>().main;
        main.startColor = color;
    }

    [Command]
    public void CmdSync()
    {
        Debug.Log("go = " + gameObject.name + " Colors[0] = " + Colors[0] + " Colors[1] = " + Colors[1]);
        foreach (var p in playerList)
        {
            var _color = Colors[playerList.IndexOf(p)];
            p.color = _color;
            p.RpcSetColor(_color);
            p.RpcOnChangeName(p.name);
            Debug.Log(p.gameObject.name + " index = " + playerList.IndexOf(p));
        }
    }

    void OnDestroy()
    {
        if (playerList != null) playerList.Remove(this);
    }

}
