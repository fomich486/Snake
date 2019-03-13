using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

namespace MatchMaker
{
    public class LanMatchMaker : NetworkManager, IMatchMaker
    {
        private const float delay = 2.5f;
        private const float repeat = 0.5f;

        private List<NetworkConnection> connections;

        [SerializeField]
        private LanCreateParameters createParams;

        private Coroutine timer;
        private Coroutine create;
        private Coroutine list;
        private bool isHost;

        public bool IsConnected { get; set; }
        public bool IsReady { get; set; }
        public IMatchCreateParameters CreateParameters { get { return createParams; } set { createParams = value as LanCreateParameters; } }

        public NetworkClient NetworkClient { get; private set; }
        public IMatchJoinParameters JoinParameters { get; set; }
        public IMatchListParameters ListParameters { get; set; }
        public IMatchLeaveParameters LeaveParameters { get; set; }
        public IMatchMakerStartParameters StartParameters { get; set; }
        public int PlayerConnectedCount { get; set; }

        public event EventHandler Connected;
        public event EventHandler MatchCreated;
        public event EventHandler MatchJoined;
        public event EventHandler MatchLeft;
        public event EventHandler MatchListGot;

        public void CreateMatch(IMatchCreateParameters _parameters = null)
        {
            var _data = CreateParameters as LanCreateParameters;
            if (NetworkClient.active) StopHost();
            NetworkClient = StartHost();
            LANBroadcastService.Instance.StopBroadCasting();
            LANBroadcastService.Instance.StartAnnounceBroadCasting();
            LANBroadcastService.Instance.AdditionalData = JsonUtility.ToJson(new LanMatchInfo("", _data.AppId, _data.PlayersCount));
            //create = StartCoroutine(CreateCoroutine());
        }

        //private IEnumerator CreateCoroutine()
        //{
        //    Debug.Log("Start broadcast");
        //    while (!LanNetworkDiscovery.Instance.running && LanNetworkDiscovery.Instance.StartBroadcasting())
        //    {
        //        yield return new WaitForSeconds(repeat);
        //    }
        //    NetworkClient = StartHost();

        //}

        public void Init()
        {

        }

        public bool IsOkToJoin(IMatchJoinParameters _localparameters, IMatchInfo _matchInfo)
        {
            var _data = CreateParameters as LanCreateParameters;
            var _model = (_matchInfo as LanMatchInfo);
            return _model.AppId.Equals(_data.AppId);
        }

        public void JoinMatch(IMatchJoinParameters _parameters = null)
        {
            NetworkClient = StartClient();
        }

        public void JoinMatch(IMatchInfo _parameters)
        {
            var _responce = _parameters as LanMatchInfo;
            networkAddress = _responce.FromAddress;
            if (NetworkClient.active) StopClient();
            NetworkClient = StartClient();
            LANBroadcastService.Instance.StopBroadCasting();
            //Debug.Log("StartClient! Connect to " + _responce.FromAddress);
            //LanNetworkDiscovery.Instance.Invoke("StopBroadcasting",0.1f);
        }

        public void LeaveMatch(IMatchLeaveParameters _parameters = null)
        {
            NetworkClient.Disconnect();
            StopMatchmaker();
        }

        public void ListMatches(IMatchListParameters _parameters = null)
        {
            LANBroadcastService.Instance.StartSearchBroadCasting(Found, CreateNew);
            //timer = StartCoroutine(TimerCoroutine(delay));
            //list = StartCoroutine(ListCoroutine());
        }

        private void CreateNew()
        {
            if (MatchListGot != null)
            {
                MatchListGot(this, EventArgs.Empty);
            }
        }

        private void Found(string strIP, string message)
        {
            OnServerDetected(strIP, message);
        }

        //private IEnumerator ListCoroutine()
        //{
        //    Debug.Log("Start Reecieve");

        //    while (!LanNetworkDiscovery.Instance.ReceiveBraodcast())
        //    {
        //        Debug.Log("recieving");
        //        yield return new WaitForSeconds(repeat);
        //    }
        //}

        private IEnumerator TimerCoroutine(float _delay)
        {
            yield return new WaitForSeconds(_delay + UnityEngine.Random.Range(0f, 3f));
            if (!IsConnected)
            {
                //LanNetworkDiscovery.Instance.InitializeNetworkDiscovery();
                if (MatchListGot != null)
                {
                    MatchListGot(this, EventArgs.Empty);
                }
            }
            Debug.Log("Time out!");
        }

        private void OnServerDetected(string _fromAddress, string _data)
        {
            //LanNetworkDiscovery.Instance.onServerDetected -= OnServerDetected;
            OnListMatches(new LanListResponce(_fromAddress, _data));
        }

        public void OnConnected(IMatchConnectResponce _responce)
        {
            throw new NotImplementedException();
        }

        public void OnCreateMatch(IMatchCreateResponse _responce)
        {
            if (MatchCreated != null) MatchCreated(this, EventArgs.Empty);
        }

        public void OnJoinMatch(IMatchJoinResponce _responce)
        {
            //StopCoroutine(timer);
            IsConnected = true;
        }

        public void OnLeaveMatch(IMatchLeaveResponce _responce)
        {
            if (MatchLeft != null) MatchLeft(this, EventArgs.Empty);
        }

        public void OnListMatches(IMatchListResponce _responce)
        {
            if (MatchListGot != null) MatchListGot(this, _responce as LanListResponce);
        }

        public void SetPlayersCountToStartGame(int _count)
        {

        }

        public void StartMatchMaker(IMatchMakerStartParameters _parameters = null)
        {
            connections = new List<NetworkConnection>();
        }

        // Use this for initialization
        void Start()
        {
            ClientScene.RegisterPrefab(playerPrefab);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            isHost = true;
            OnCreateMatch(null);
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            if (!conn.isReady)
            {
                base.OnClientConnect(conn);
            }
            OnJoinMatch(null);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            OnLeaveMatch(null);
            Debug.Log("ClientDisconnect");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            connections.Remove(conn);
            PlayerConnectedCount--;
            Debug.Log("ServerDisconnect");
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            if (++PlayerConnectedCount > MatchMakingManager.Instance.PlayersToStart || connections.ToList().Exists(p => p.address == conn.address))
            {
                conn.Disconnect();
            }
            else
            {
                base.OnServerAddPlayer(conn, playerControllerId);
                connections.Add(conn);
                Debug.Log("OnServerAddPlayer");
            }

            if(PlayerConnectedCount == MatchMakingManager.Instance.PlayersToStart)
            {
                if (Connected != null) Connected(this, EventArgs.Empty);
            }
        }

        public void StopMatchmaker()
        {
            Debug.Log("StopMatchmakerCalled");
            PlayerConnectedCount = 0;
            LANBroadcastService.Instance.StopBroadCasting();
            Debug.Log("LANBroadcastService.Instance.StopBroadCasting();");
            if (NetworkClient!=null&&NetworkClient.isConnected)
            {
                if (isHost)
                {
                    StopHost();
                    Debug.Log("StopHost");
                }
                else
                {
                    Debug.Log("StopCLient");
                    StopClient();
                }
            }
            IsConnected = false;
            isHost = false;
        }
    }
}
