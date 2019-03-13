using UnityEngine;
using System.Collections;
using NetworkExtension;
using System;

public class TransportLevelTest :INetworkTransportLevel
{ 
    public Action<ServerCommands, string> Recieved  { get; set; }
    public void Send(string _jsdata)
    {
        var _id = JsonUtility.FromJson<Command>(_jsdata).Id;
        Recieved((ServerCommands)_id, JsonUtility.ToJson(new Vector2(1, 2)));
    }
}
