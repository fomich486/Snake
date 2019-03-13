using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;

using System;

namespace MatchMaker
{
    public class LobbyManager : NetworkLobbyManager, IMatchMaker
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;




        [SerializeField]
        private MatchCreateParametersUNET createParams;
        [SerializeField]
        private MatchJoinParametersUNET joinParams;
        [SerializeField]
        private MatchListParametersUNET listParams;
        [SerializeField]
        private MatchLeaveParametersUNET leaveParams;

        public int PlayerConnectedCount { get; set; }
        public event EventHandler Connected;
        public event EventHandler MatchCreated;
        public event EventHandler MatchJoined;
        public event EventHandler MatchLeft;
        public event EventHandler MatchListGot;

        public bool IsConnected
        {
            get
            {
                return _isMatchmaking;
            }
        }

        public bool IsReady { get; set; }
        public IMatchCreateParameters CreateParameters { get { return createParams; } set { createParams = value as MatchCreateParametersUNET; } }
        public IMatchJoinParameters JoinParameters { get { return joinParams; } set { joinParams = value as MatchJoinParametersUNET; } }
        public IMatchListParameters ListParameters { get { return listParams; } set { listParams = value as MatchListParametersUNET; } }
        public IMatchLeaveParameters LeaveParameters { get { return leaveParams; } set { leaveParams = value as MatchLeaveParametersUNET; } }
        public IMatchMakerStartParameters StartParameters { get; set; }



        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;

        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        void Start()
        {
            s_Singleton = this;
            _lobbyHooks = GetComponent<LobbyHook>();
            SetPlayersCountToStartGame(MatchMakingManager.Instance.PlayersToStart);


        }

        public void SetPlayersCountToStartGame(int _count)
        {
            MatchMakingManager.Instance.PlayersToStart = _count;
            (CreateParameters as MatchCreateParametersUNET).MatchSize = MatchMakingManager.Instance.PlayersToStart;
            (CreateParameters as MatchCreateParametersUNET).MaxPlayers = MatchMakingManager.Instance.PlayersToStart;
            (CreateParameters as MatchCreateParametersUNET).MinPlayers = MatchMakingManager.Instance.PlayersToStart;
            maxPlayers = (CreateParameters as MatchCreateParametersUNET).MaxPlayers;
            minPlayers = (CreateParameters as MatchCreateParametersUNET).MinPlayers;
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (MatchMakingManager.Instance.ShowUI)
                {
                    if (MatchMakerUIControllerBase.Instance.TopPanel.isInGame)
                    {
                        MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.lobbyPanel);
                        if (_isMatchmaking)
                        {
                            if (conn.playerControllers[0].unetView.isServer)
                            {
                                MatchMakerUIControllerBase.Instance.backDelegate = StopHostClbk;
                            }
                            else
                            {
                                MatchMakerUIControllerBase.Instance.backDelegate = StopClientClbk;
                            }
                        }
                        else
                        {
                            if (conn.playerControllers[0].unetView.isClient)
                            {
                                MatchMakerUIControllerBase.Instance.backDelegate = StopHostClbk;
                            }
                            else
                            {
                                MatchMakerUIControllerBase.Instance.backDelegate = StopClientClbk;
                            }
                        }
                    }
                    else
                    {
                        MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
                    }

                    MatchMakerUIControllerBase.Instance.TopPanel.ToggleVisibility(true);
                    MatchMakerUIControllerBase.Instance.TopPanel.isInGame = false;
                }
                else
                {
                    MatchMakerUIControllerBase.Instance.ChangeTo(null);

                    Destroy(GameObject.Find("MainMenuUI(Clone)"));

                    //backDelegate = StopGameClbk;
                    MatchMakerUIControllerBase.Instance.TopPanel.isInGame = true;
                    MatchMakerUIControllerBase.Instance.TopPanel.ToggleVisibility(false);
                }
                if (!MatchMakerUIControllerBase.Instance.TopPanel.isInGame)
                {
                    _isMatchmaking = false;
                }
            }
        }


        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
            }
            _isMatchmaking = false;
        }

        public void StopHostClbk()
        {
            if (_isMatchmaking)
            {
                matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
                _disconnectServer = true;
            }
            else
            {
                StopHost();
            }


            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
            }
            _isMatchmaking = false;
        }

        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
            }
            _isMatchmaking = false;
        }

        public void StopServerClbk()
        {
            StopServer();
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
            }
            _isMatchmaking = false;
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }




        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.InfoPanel.Display("Kicked by Server", "Close", null);
            }
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.lobbyPanel);
                MatchMakerUIControllerBase.Instance.backDelegate = StopHostClbk;
                MatchMakerUIControllerBase.Instance.SetServerInfo("Hosting", networkAddress);
            }
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            OnCreateMatch(new MatchCreateResponceUNET(success, extendedInfo, matchInfo));
        }

        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
           // foreach (PlayerController p in ClientScene.localPlayers)
              //  localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
            }

        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("OnLobbyServerCreateLobbyPlayer");
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;


            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.
            Debug.Log("OnLobbyServerSceneLoadedForPlayer");
            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
            Debug.Log("OnLobbyServerPlayersReady");
            bool allready = false;
            var _readycount = 0;
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    if (lobbySlots[i].readyToBegin)
                    {
                        _readycount++;
                    }
                    if ( _readycount == MatchMakingManager.Instance.PlayersToStart)
                    {
                        allready = true;
                    }
                }

            }
            if (allready)
                StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            Debug.Log("ServerCountdownCoroutine");
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                    (lobbySlots[i] as LobbyPlayer).RpcMatchStarted();
                }
            }
        }



        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("OnClientConnect");
            base.OnClientConnect(conn);
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.InfoPanel.gameObject.SetActive(false);
            }
            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {
                if (MatchMakingManager.Instance.ShowUI)
                {
                    MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.lobbyPanel);
                    MatchMakerUIControllerBase.Instance.backDelegate = StopClientClbk;
                    MatchMakerUIControllerBase.Instance.SetServerInfo("Client", networkAddress);
                }
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
            }
            _isMatchmaking = false;
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
                MatchMakerUIControllerBase.Instance.InfoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
            }
            _isMatchmaking = false;
        }


        ///

        public void StartMatchMaker(IMatchMakerStartParameters _parameters)
        {
            StartMatchMaker();
        }

        public void Init()
        {

        }

        public void CreateMatch(IMatchCreateParameters _parameters = null)
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.backDelegate = StopHost;
                MatchMakerUIControllerBase.Instance.SetServerInfo("Matchmaker Host", matchHost);
            }
            _isMatchmaking = true;
            if (_parameters == null) _parameters = CreateParameters;
            var _p = (MatchCreateParametersUNET)_parameters;
            Debug.Log("Create params: " + JsonUtility.ToJson(_p));
            matchMaker.CreateMatch(_p.MatchName, (uint)_p.MatchSize, _p.MatchAdvertize, _p.MatchPassword, _p.PulicClientAddress, _p.PrivateClientAddress, _p.EloScoreForMatch, _p.RequestDomain, (success, extendedInfo, responseData) => { OnCreateMatch(new MatchCreateResponceUNET(success, extendedInfo, responseData)); });
        }

        public void JoinMatch(IMatchJoinParameters _parameters = null)
        {
            _isMatchmaking = true;
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.backDelegate = StopClientClbk;
            }
            if (_parameters == null) _parameters = JoinParameters;
            var _p = (MatchJoinParametersUNET)_parameters;
            matchMaker.JoinMatch((UnityEngine.Networking.Types.NetworkID)_p.NetId, _p.MatchPassword, _p.PulicClientAddress, _p.PrivateClientAddress, _p.EloScoreForClient, _p.RequestDomain, (success, extendedInfo, responseData) => { OnJoinMatch(new MatchJoinResponceUNET(success, extendedInfo, responseData)); });
        }

        public void JoinMatch(IMatchInfo _parameters)
        {
            var _matchInfo = (_parameters as MatchInfoUNET).Info;
            (JoinParameters as MatchJoinParametersUNET).NetId = (ulong)_matchInfo.networkId;
            JoinMatch();
        }

        public void LeaveMatch(IMatchLeaveParameters _parameters = null)
        {
            var _p = LeaveParameters as MatchLeaveParametersUNET;
            _p.NetId = (JoinParameters as MatchJoinParametersUNET).NetId;
            matchMaker.DropConnection((UnityEngine.Networking.Types.NetworkID)_p.NetId, _p.NodeId, _p.RequestDomain, (success, extendedInfo) => { OnLeaveMatch(new MatchLeaveResponceUNET(success, extendedInfo)); });
        }

        public void ListMatches(IMatchListParameters _parameters = null)
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.backDelegate = SimpleBackClbk;
            }
            if (_parameters == null)
            {
                _parameters = ListParameters;
            }
            var _p = (MatchListParametersUNET)_parameters;
            Debug.Log("List params: " + JsonUtility.ToJson(_p));
            matchMaker.ListMatches(_p.StartPageNumer, _p.ResultPageSize, _p.MatchNameFilter, _p.FilterOutPrivateMatchesFromResults, _p.EloScoreTarget, _p.RequestDomain, (success, extendedInfo, responseData) => { OnListMatches(new MatchListResponceUNET(success, extendedInfo, responseData)); });
        }

        public void OnConnected(IMatchConnectResponce _responce)
        {
            var _res = _responce as MatchConnectResponceUNET;
            //ServerChangeScene(playScene);
            if (Connected != null) Connected(this, _res);
        }

        public void OnCreateMatch(IMatchCreateResponse _responce)
        {
            var _res = _responce as MatchCreateResponceUNET;
            base.OnMatchCreate(_res.Success, _res.ExtendedInfo, _res.ResponseData);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
            if (MatchCreated != null) MatchCreated(this, _res);
        }

        public void OnJoinMatch(IMatchJoinResponce _responce)
        {
            var _res = _responce as MatchJoinResponceUNET;
            OnMatchJoined(_res.Success, _res.ExtendedInfo, _res.ResponseData);
            if (MatchJoined != null) MatchJoined(this, _res);
        }

        public void OnLeaveMatch(IMatchLeaveResponce _responce)
        {
            if (MatchMakingManager.Instance.ShowUI)
            {
                MatchMakerUIControllerBase.Instance.ChangeTo(MatchMakerUIControllerBase.Instance.mainMenuPanel);
            }
        }

        public void OnListMatches(IMatchListResponce _responce)
        {
            var _model = (MatchListResponceUNET)_responce;
            if (MatchListGot != null) MatchListGot(this, _model);
        }

        public bool IsOkToJoin(IMatchJoinParameters _localparameters, IMatchInfo _matchInfo)
        {
            var _match = _matchInfo as MatchInfoUNET;
            var _joinparams = _localparameters as MatchJoinParametersUNET;
            return (_match.Info.maxSize == MatchMakingManager.Instance.PlayersToStart && _joinparams.EloScoreForClient == _match.Info.averageEloScore);
        }

        void OnDestroy()
        {
            Debug.Log("Destroing");
        }
    }
}
