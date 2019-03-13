using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace MatchMaker
{
    [Serializable]
    public class MatchMakerUnity : NetworkBehaviour, IMatchMaker
    {
        public bool IsConnected { get; set; }
        public bool IsReady { get; set; }

        public MatchInfo CurrentMatchInfo { get; private set; }
        private MatchMakerUNETHelper unetHelper;

        public List<MatchInfoSnapshot> ListOfMatches;

        public int PlayerConnectedCount { get; set; }
        public event EventHandler Connected;
        public event EventHandler MatchCreated;
        public event EventHandler MatchJoined;
        public event EventHandler MatchLeft;
        public event EventHandler MatchListGot;
        public event EventHandler ClientConnected;

        [SerializeField]
        public IMatchCreateParameters CreateParameters { get; set; }
        [SerializeField]
        public IMatchJoinParameters JoinParameters { get; set; }
        [SerializeField]
        public IMatchListParameters ListParameters { get; set; }
        [SerializeField]
        public IMatchLeaveParameters LeaveParameters { get; set; }
        [SerializeField]
        public IMatchMakerStartParameters StartParameters { get; set; }


        private void Awake()
        {
            if (NetworkManager.singleton == null)
            {
                unetHelper = gameObject.AddComponent<MatchMakerUNETHelper>();
                unetHelper.autoCreatePlayer = false;
            }
        }
        // Use this for initialization
        private void Start()
        {
            StartMatchMaker(null);
            unetHelper.NetworkMatch = NetworkManager.singleton.matchMaker;
            unetHelper.ClientConnected += OnClientConnected;
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void Init()
        {
            CreateParameters = new MatchCreateParametersUNET("New Match", 2, true, "", "", "", 0, 0);
            JoinParameters = new MatchJoinParametersUNET(UnityEngine.Networking.Types.NetworkID.Invalid, "", "", "", 0, 0);
            ListParameters = new MatchListParametersUNET(0, 200, "", false, 0, 0);
            LeaveParameters = new MatchLeaveParametersUNET(UnityEngine.Networking.Types.NetworkID.Invalid, UnityEngine.Networking.Types.NodeID.Invalid, 0);
        }


        public void StartMatchMaker(IMatchMakerStartParameters _parameters)
        {
            unetHelper.StartMatchMaker(_parameters);
        }

        public void ListMatches(IMatchListParameters _parameters = null)
        {
            unetHelper.MatchListGot += OnListMatches;
            unetHelper.ListMatches(ListParameters);
        }

        public void CreateMatch(IMatchCreateParameters _parameters = null)
        {
            if (CurrentMatchInfo == null)
            {
                unetHelper.MatchCreated += OnCreateMatch;
                unetHelper.CreateMatch(CreateParameters);               
            }
            else
            {
                Debug.Log("You already in match");
            }
        }

        public void JoinMatch(IMatchJoinParameters _parameters = null)
        {
            if (CurrentMatchInfo == null)
            {
                unetHelper.MatchJoined += OnJoinMatch;
                unetHelper.JoinMatch(JoinParameters);
            }
        }

        public void JoinMatch(IMatchInfo _parameters)
        {
            var _matchInfo = (_parameters as MatchInfoUNET).Info;
            (JoinParameters as MatchJoinParametersUNET).NetId = (ulong)_matchInfo.networkId;
            JoinMatch();
        }

        public void LeaveMatch(IMatchLeaveParameters _parameters = null)
        {
            if (CurrentMatchInfo != null)
            {
                unetHelper.MatchLeft += OnLeaveMatch;
                var _params = LeaveParameters as MatchLeaveParametersUNET;
                _params.NetId = (ulong)CurrentMatchInfo.networkId;
                _params.NodeId = CurrentMatchInfo.nodeId;
                _params.RequestDomain = CurrentMatchInfo.domain;
                Debug.Log("LeaveParams => " + JsonUtility.ToJson(_params));
                unetHelper.LeaveMatch(LeaveParameters);
            }
        }

        private void OnCreateMatch(object sender, EventArgs e)
        {
            unetHelper.MatchCreated -= OnCreateMatch;
            var _responce = (MatchCreateResponceUNET)e;
            if (_responce.Success)
            {
                Debug.Log("Create match succeeded");
                MatchInfo matchInfo = _responce.ResponseData;
                CurrentMatchInfo = matchInfo;
                if (!NetworkManager.singleton.isNetworkActive)
                {
                    NetworkServer.Listen(matchInfo, 9000);
                    Utility.SetAccessTokenForNetwork(matchInfo.networkId, matchInfo.accessToken);
                }
                IsConnected = true;
            }
            else
            {
                Debug.LogError("Create match failed");
            }
            OnCreateMatch(_responce);
        }

        private void OnJoinMatch(object sender, EventArgs e)
        {
            unetHelper.MatchJoined -= OnJoinMatch;
            var _responce = (MatchJoinResponceUNET)e;
            if (_responce.Success)
            {
                Debug.Log("Joined successfully; NetId = " + _responce.ResponseData.networkId);
                CurrentMatchInfo = _responce.ResponseData;
                Utility.SetAccessTokenForNetwork(CurrentMatchInfo.networkId, CurrentMatchInfo.accessToken);
                NetworkClient myClient = new NetworkClient();
                myClient.RegisterHandler(MsgType.Connect, OnConnected);
                myClient.Connect(CurrentMatchInfo);
                IsConnected = true;
            }
            else
            {
                Debug.Log("Can't join to match; Reason " + _responce.ExtendedInfo);
            }
            OnJoinMatch(_responce);
        }

        private void OnRemovePlayer(NetworkMessage netMsg)
        {
            Debug.Log("Remove player");
        }

        private void OnAddPlayer(NetworkMessage netMsg)
        {
            Debug.Log("Add player");
        }

        private void OnConnected(NetworkMessage netMsg)
        {
            Debug.Log("Connected! " + JsonUtility.ToJson(netMsg));
        }

        private void OnLeaveMatch(object sender, EventArgs e)
        {
            unetHelper.MatchLeft -= OnLeaveMatch;
            var _responce = (MatchLeaveResponceUNET)e;
            if (_responce.Success)
            {
                CurrentMatchInfo = null;
                Debug.Log("Match dropped");
                IsConnected = false;
            }
            else
            {
                Debug.LogError("Can't drop match; Reason: " + _responce.ExtendedInfo);
            }
            OnLeaveMatch(_responce);
        }

        private void OnListMatches(object sender, EventArgs e)
        {
            unetHelper.MatchListGot -= OnListMatches;
            var _responce = (MatchListResponceUNET)e;
            if (_responce.Success)
            {
                Debug.Log("Matches Count = " + _responce.ResponseData.Count);
                if (ListOfMatches == null)
                {
                    ListOfMatches = new List<MatchInfoSnapshot>();
                }
                ListOfMatches = _responce.ResponseData;
            }
            else
            {
                Debug.Log("Can't load list of matches; Reason " + _responce.ExtendedInfo);
            }
            OnListMatches(_responce);
        }

        private void OnClientConnected(object sender, EventArgs e)
        {

        }

        public void OnConnected(IMatchConnectResponce _responce)
        {
            if (Connected != null) Connected(this, (MatchConnectResponceUNET)_responce);
        }

        public void OnCreateMatch(IMatchCreateResponse _responce)
        {
            if (MatchCreated != null) MatchCreated(this, (MatchCreateResponceUNET)_responce);
        }

        public void OnJoinMatch(IMatchJoinResponce _responce)
        {
            if (MatchJoined != null) MatchJoined(this, (MatchJoinResponceUNET)_responce);
        }

        public void OnLeaveMatch(IMatchLeaveResponce _responce)
        {
            if (MatchLeft != null) MatchLeft(this, (MatchLeaveResponceUNET)_responce);
        }

        public void OnListMatches(IMatchListResponce _responce)
        {
            if (MatchListGot != null) MatchListGot(this, (MatchListResponceUNET)_responce);
        }


        public void OnClientConnected(IMatchClientInfo _responce)
        {
           
        }


        private void OnDestroy()
        {
            unetHelper.StopMatchMaker();
        }

        public bool IsOkToJoin(IMatchJoinParameters _localparameters, IMatchInfo _matchInfo)
        {
            return true;
        }

        public void SetPlayersCountToStartGame(int _count)
        {
            
        }
    }

}

