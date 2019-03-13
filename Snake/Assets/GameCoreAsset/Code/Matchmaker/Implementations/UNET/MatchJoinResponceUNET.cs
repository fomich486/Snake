using System;
using UnityEngine.Networking.Match;

namespace MatchMaker
{
    public class MatchJoinResponceUNET : EventArgs, IMatchJoinResponce
    {
        public bool Success;
        public string ExtendedInfo;
        public MatchInfo ResponseData;

        public MatchJoinResponceUNET(bool _success, string _extendedInfo, MatchInfo _responseData)
        {
            Success = _success;
            ExtendedInfo = _extendedInfo;
            ResponseData = _responseData;
        }
    }
}