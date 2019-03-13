using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace MatchMaker
{
    public class LanNetworkDiscovery : NetworkDiscovery
    {

        public static LanNetworkDiscovery Instance;

        public Action<string, string> onServerDetected;

        void OnServerDetected(string fromAddress, string data)
        {
            Debug.Log("OnServerDetected "+ fromAddress);
            if (onServerDetected != null)
            {
                onServerDetected.Invoke(fromAddress, data);
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }


        void Start()
        {
            //Debug.Log("InitializeNetworkDiscovery");
            //InitializeNetworkDiscovery();
        }

        public bool InitializeNetworkDiscovery()
        {
            if (running)
            {
                StopBroadcasting();
            }
            return Initialize();
        }

        public bool StartBroadcasting()
        {
            if (!running)
            {
                if (!InitializeNetworkDiscovery())
                {
                    return false;
                }
            }
            return StartAsServer();
        }

        public bool ReceiveBraodcast()
        {
            if (!running)
            {
                if (!InitializeNetworkDiscovery())
                {
                    return false;
                }
            }
            return StartAsClient();
        }

        public void StopBroadcasting()
        {
            Debug.Log("StopBroadcasting");
            if (running)
            {
                StopBroadcast();
            }
        }

        public void SetBroadcastData(string broadcastPayload)
        {
            broadcastData = broadcastPayload;
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            Debug.Log("Recieve broadcast from " + fromAddress);
            OnServerDetected(fromAddress.Split(':')[3], data);
        }


    }
}
