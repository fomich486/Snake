using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NetworkExtension
{
    public class AsyncNetworkWorker
    {
        private static AsyncNetworkWorker instance;
        private Dictionary<int, List<Action<string>>> listenDictionary;
        private INetworkTransportLevel networkTransport;

        private AsyncNetworkWorker()
        {   if(networkTransport == null)
            {
                networkTransport = new TransportLevelTest();
            }
            networkTransport.Recieved += OnRecieve;
            listenDictionary = new Dictionary<int, List<Action<string>>>();
        }

        ~AsyncNetworkWorker()
        {
            networkTransport.Recieved -= OnRecieve;
        }

        public static AsyncNetworkWorker Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AsyncNetworkWorker();
                }
                return instance;
            }
        }

        public void Send(Command _command)
        {
            var _jsdata = JsonUtility.ToJson(_command);
            networkTransport.Send(_jsdata);
        }

        public void OnRecieve(ServerCommands _id, string _jsdata)
        {
            var actions = GetActions((int)_id);
            if (actions != null)
            {
                foreach (var action in actions)
                {
                   action(_jsdata);
                }
            }
        }

        public void RegisterListener(ServerCommands id, Action<string> _action)
        {
            if (listenDictionary.ContainsKey((int)id))
            {
                if (!listenDictionary[(int)id].Contains(_action)) listenDictionary[(int)id].Add(_action);
            }
            else
            {
                listenDictionary.Add((int)id, new List<Action<string>>() { _action });
            }
        }

        public void UnregisterListener(ServerCommands id, Action<string> _action)
        {
            if (listenDictionary.ContainsKey((int)id))
            {
                if (listenDictionary[(int)id].Contains(_action)) listenDictionary[(int)id].Remove(_action);
            }
        }

        public void UnregisterAllListenerOnCommand(ServerCommands _id)
        {
            if (listenDictionary.ContainsKey((int)_id))
            {
                listenDictionary[(int)_id].Clear();
            }
        }

        public void UnregisterAll()
        {
            listenDictionary.Clear();
        }

        private List<Action<string>> GetActions(int _id)
        {
            List<Action<string>> _list;
            listenDictionary.TryGetValue(_id, out _list);
            return _list;
        }

        public void SetNetorkTransport(INetworkTransportLevel _transport)
        {
            networkTransport = _transport;
        }
    }
}


