using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace MatchMaker
{

    public class MatchMakerUNETHelper : NetworkManager, IMatchMaker
    {
        public bool IsConnected { get; set; }
        public bool IsReady { get; set; }

        public NetworkMatch NetworkMatch;
        private MatchCreateParametersUNET matchparameters;
        private MatchListParametersUNET _listParameters;


        static short MsgKicked = MsgType.Highest + 1;

        public int PlayerConnectedCount { get; set; }
        public event EventHandler Connected;
        public event EventHandler MatchCreated;
        public event EventHandler MatchJoined;
        public event EventHandler MatchLeft;
        public event EventHandler MatchListGot;
        public event EventHandler ClientConnected;

        public IMatchCreateParameters CreateParameters { get; set; }
        public IMatchJoinParameters JoinParameters { get; set; }
        public IMatchListParameters ListParameters { get; set; }
        public IMatchLeaveParameters LeaveParameters { get; set; }
        public IMatchMakerStartParameters StartParameters { get; set; }


        public void StartMatchMaker(IMatchMakerStartParameters _parameters)
        {
            NetworkManager.singleton.StartMatchMaker();
        }

        public void Init()
        {

        }

        #region Requests

        public void CreateMatch(IMatchCreateParameters _parameters)
        {
            var _p = (MatchCreateParametersUNET)_parameters;
            Debug.Log("Create params: " + JsonUtility.ToJson(_p));
            NetworkMatch.CreateMatch(_p.MatchName, (uint)_p.MatchSize, _p.MatchAdvertize, _p.MatchPassword, _p.PulicClientAddress, _p.PrivateClientAddress, _p.EloScoreForMatch, _p.RequestDomain, (success, extendedInfo, responseData) => { OnCreateMatch(new MatchCreateResponceUNET(success, extendedInfo, responseData)); });
        }

        public void JoinMatch(IMatchJoinParameters _parameters)
        {
            var _p = (MatchJoinParametersUNET)_parameters;
            NetworkMatch.JoinMatch((UnityEngine.Networking.Types.NetworkID)_p.NetId, _p.MatchPassword, _p.PulicClientAddress, _p.PrivateClientAddress, _p.EloScoreForClient, _p.RequestDomain, (success, extendedInfo, responseData) => { OnJoinMatch(new MatchJoinResponceUNET(success, extendedInfo, responseData)); });
        }

        public void JoinMatch(IMatchInfo _parameters)
        {

        }

        public void LeaveMatch(IMatchLeaveParameters _parameters)
        {
            var _p = (MatchLeaveParametersUNET)_parameters;
            NetworkMatch.DropConnection((UnityEngine.Networking.Types.NetworkID)_p.NetId, _p.NodeId, _p.RequestDomain, (success, extendedInfo) => { OnLeaveMatch(new MatchLeaveResponceUNET(success, extendedInfo)); });
        }

        public void ListMatches(IMatchListParameters _parameters)
        {
            var _p = (MatchListParametersUNET)_parameters;
            Debug.Log("List params: " + JsonUtility.ToJson(_p));
            NetworkMatch.ListMatches(_p.StartPageNumer, _p.ResultPageSize, _p.MatchNameFilter, _p.FilterOutPrivateMatchesFromResults, _p.EloScoreTarget, _p.RequestDomain, (success, extendedInfo, responseData) => { OnListMatches(new MatchListResponceUNET(success, extendedInfo, responseData)); });
        }
        #endregion



        #region Responces

        public void OnCreateMatch(IMatchCreateResponse _responce)
        {
            var _model = (MatchCreateResponceUNET)_responce;
            NetworkServer.Listen(_model.ResponseData, 9000);
            if (MatchCreated != null) MatchCreated(this, _model);
        }

        public void OnListMatches(IMatchListResponce _responce)
        {
            var _model = (MatchListResponceUNET)_responce;
            if (MatchListGot != null) MatchListGot(this, _model);
        }

        public void OnJoinMatch(IMatchJoinResponce _responce)
        {
            var _model = (MatchJoinResponceUNET)_responce;
            NetworkManager.singleton.StartClient(_model.ResponseData);
            if (MatchJoined != null) MatchJoined(this, _model);
        }

        public void OnLeaveMatch(IMatchLeaveResponce _responce)
        {
            var _model = (MatchLeaveResponceUNET)_responce;
            if (MatchLeft != null) MatchLeft(this, _model);
        }


        public void OnConnected(IMatchConnectResponce _responce)
        {
            var _model = (MatchConnectResponceUNET)_responce;
            if (Connected != null) Connected(this, _model);
        }

        public bool IsOkToJoin(IMatchJoinParameters _localparameters, IMatchInfo _matchInfo)
        {
            return true;
        }

        public void SetPlayersCountToStartGame(int _count)
        {
            
        }
        #endregion

        #region NetworkManager


        //===================
        #endregion
    }


}

