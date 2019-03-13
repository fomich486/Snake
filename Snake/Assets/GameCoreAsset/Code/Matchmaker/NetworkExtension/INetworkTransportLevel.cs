using System;

namespace NetworkExtension
{
    public interface INetworkTransportLevel
    {
        Action<ServerCommands ,string> Recieved { get; set; }
        void Send(string _jsdata);
    }
}