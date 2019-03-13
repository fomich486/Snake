using UnityEngine;
using System.Collections;
using System;

namespace MatchMaker
{
    [RequireComponent(typeof(TurboWWWManager))]
    public class WWWBackEndMatchMaker : MonoBehaviour, IMatchMaker
    {
        public bool IsConnected { get; set; }
     
        public bool IsReady { get  ; set ; }


        [SerializeField]
        private WWWCreateParameters createParams;

        [SerializeField]
        private WWWJoinParameters joinParams;

        [SerializeField]
        private WWWListParameters listParams;

        [SerializeField]
        private WWWLeaveParameters leaveParams;


        public IMatchCreateParameters CreateParameters { get { return createParams; }  set { createParams = value as WWWCreateParameters; }  }
        public IMatchJoinParameters JoinParameters { get { return joinParams; } set { joinParams = value as WWWJoinParameters; } }
        public IMatchListParameters ListParameters { get { return listParams; } set { listParams = value as WWWListParameters; } }
        public IMatchLeaveParameters LeaveParameters { get { return leaveParams; } set { leaveParams = value as WWWLeaveParameters; } }
        public IMatchMakerStartParameters StartParameters { get; set; }
        public int PlayerConnectedCount { get; set; }

        public event EventHandler Connected;
        public event EventHandler MatchCreated;
        public event EventHandler MatchJoined;
        public event EventHandler MatchLeft;
        public event EventHandler MatchListGot;

       

        public void CreateMatch(IMatchCreateParameters _parameters = null)
        {
            if (_parameters == null) _parameters = CreateParameters;
            var _url = (_parameters as WWWCreateParameters).CreationUrl;
            TurboWWWSyncWrapper.Instance.Get(_url, (s) => OnCreateMatch(new WWWMatchCreateResponce(s)), (s)=>Debug.Log("Creation error " + s));
        }

        public void Init()
        {
            
        }

        public bool IsOkToJoin(IMatchJoinParameters _localparameters, IMatchInfo _matchInfo)
        {
            return true;
        }

        public void JoinMatch(IMatchJoinParameters _parameters = null)
        {
            if (_parameters == null) _parameters = JoinParameters;
            var _url = (_parameters as WWWCreateParameters).CreationUrl;
            TurboWWWSyncWrapper.Instance.Get(_url, (s) => OnJoinMatch(new WWWMatchJoinResponce(s)), (s) => Debug.Log("Join error " + s));
        }

        public void JoinMatch(IMatchInfo _parameters)
        {
            var _url = (_parameters as WWWCreateParameters).CreationUrl;
            TurboWWWSyncWrapper.Instance.Get(_url, (s) => OnJoinMatch(new WWWMatchJoinResponce(s)), (s) => Debug.Log("Join error " + s));
        }

        public void LeaveMatch(IMatchLeaveParameters _parameters = null)
        {
            if (_parameters == null) _parameters = LeaveParameters;
            var _url = (_parameters as WWWLeaveParameters).LeaveUrl;
            TurboWWWSyncWrapper.Instance.Get(_url, (s) => OnLeaveMatch(new WWWMatchLeaveResponce(s)), (s) => Debug.Log("Leave error " + s));
        }

        public void ListMatches(IMatchListParameters _parameters = null)
        {
            if (_parameters == null) _parameters = ListParameters;
            var _url = (_parameters as WWWListParameters).ListUrl;
            var _jsData = (_parameters as WWWListParameters).FilterData;
            TurboWWWSyncWrapper.Instance.Post(_url, _jsData, (s) => OnListMatches(new WWWMatchListResponce(s)), (s) => Debug.Log("List error " + s));
        }

        public void OnConnected(IMatchConnectResponce _responce)
        {
            if (Connected != null) Connected(this, _responce as WWWMatchStartResponce);
        }

        public void OnCreateMatch(IMatchCreateResponse _responce)
        {
            if (MatchCreated != null) MatchCreated(this, _responce as WWWMatchCreateResponce);
        }

        public void OnJoinMatch(IMatchJoinResponce _responce)
        {
            if (MatchJoined != null) MatchJoined(this, _responce as WWWMatchCreateResponce);
        }

        public void OnLeaveMatch(IMatchLeaveResponce _responce)
        {
            if (MatchLeft != null) MatchLeft(this, _responce as WWWMatchLeaveResponce);
        }

        public void OnListMatches(IMatchListResponce _responce)
        {
            if (MatchListGot != null) MatchListGot(this, _responce as WWWMatchListResponce);
        }

        public void SetPlayersCountToStartGame(int _count)
        {

        }

        public void StartMatchMaker(IMatchMakerStartParameters _parameters = null)
        {
            
        }

        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }



        public enum GameTypes
        {
            Default,
            Normal,
            Hardkore
        }
    }
}