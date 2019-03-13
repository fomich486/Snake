using System;
using UnityEngine.Networking.Match;

namespace MatchMaker
{
    /// <summary>
    /// Matchmaker interface. Contains base methods and events signatures to implement matchmaking system into a project
    /// </summary>
    public interface IMatchMaker
    {
        bool IsConnected { get; }
        bool IsReady { get; set; }

        IMatchCreateParameters CreateParameters { get; set; }
        IMatchJoinParameters JoinParameters { get; set; }
        IMatchListParameters ListParameters { get; set; }
        IMatchLeaveParameters LeaveParameters { get; set; }
        IMatchMakerStartParameters StartParameters { get; set; }
        int PlayerConnectedCount { get; set; }

        /// <summary>
        /// Events
        /// </summary>
        event EventHandler Connected;
        event EventHandler MatchCreated;
        event EventHandler MatchJoined;
        event EventHandler MatchLeft;
        event EventHandler MatchListGot;

        /// <summary>
        /// Init Matchmaker Module instance
        /// </summary>
        /// <param name="_parameters"></param>
        void StartMatchMaker(IMatchMakerStartParameters _parameters = null);

        /// <summary>
        /// Init default parameters
        /// </summary>
        void Init();

        /// <summary>
        /// Creates match with parameters
        /// </summary>
        /// <param name="_parameters">Custom parameters. Should implementing IMatchCreateParameters interface</param>
        void CreateMatch(IMatchCreateParameters _parameters = null);

        /// <summary>
        /// Join match with parameters
        /// </summary>
        /// <param name="_parameters">Custom parameters. Should implementing IMatchJoinParameters interface</param>
        void JoinMatch(IMatchJoinParameters _parameters = null);


        /// <summary>
        /// Join match with parameters
        /// </summary>
        /// <param name="_parameters">Gotten match info parameters. Should implementing IMatchInfo interface</param>
        void JoinMatch(IMatchInfo _parameters);

        /// <summary>
        /// Leave match with parameters
        /// </summary>
        /// <param name="_parameters">Custom parameters. Should implementing IMatchLeaveParameters interface</param>
        void LeaveMatch(IMatchLeaveParameters _parameters = null);

        /// <summary>
        /// Get list of created matches
        /// </summary>
        /// <param name="_parameters">Custom parameters. Should implementing IMatchLeaveParameters interface</param>
        void ListMatches(IMatchListParameters _parameters = null);

        /// <summary>
        /// Method that fires on start of match
        /// </summary>
        /// <param name="_responce"></param>
        void OnConnected(IMatchConnectResponce _responce);

        /// <summary>
        /// Method that fires on successful match creation result
        /// </summary>
        /// <param name="_responce"></param>
        void OnCreateMatch(IMatchCreateResponse _responce);

        /// <summary>
        /// Method that fires on joining match result
        /// </summary>
        /// <param name="_responce"></param>
        void OnJoinMatch(IMatchJoinResponce _responce);

        /// <summary>
        /// Method that fires on leaving match result
        /// </summary>
        /// <param name="_responce"></param>
        void OnLeaveMatch(IMatchLeaveResponce _responce);


        /// <summary>
        /// Method that fires on  match listing result
        /// </summary>
        /// <param name="_responce"></param>
        void OnListMatches(IMatchListResponce _responce);

        /// <summary>
        /// Check if selected match matches to current join parameters 
        /// </summary>
        /// <param name="_localparameters"></param>
        /// <param name="_matchInfo"></param>
        /// <returns></returns>
        bool IsOkToJoin(IMatchJoinParameters _localparameters, IMatchInfo _matchInfo);//TODO Rename

        /// <summary>
        ///
        /// </summary>
        /// <param name="_count"></param>
        void SetPlayersCountToStartGame(int _count);
    }

}
