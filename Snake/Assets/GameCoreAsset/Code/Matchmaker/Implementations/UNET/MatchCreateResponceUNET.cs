using System;
using UnityEngine.Networking.Match;

namespace MatchMaker
{
    public class MatchCreateResponceUNET : EventArgs, IMatchCreateResponse
    {
        public bool Success;
        public string ExtendedInfo;
        public MatchInfo ResponseData;

        public MatchCreateResponceUNET(bool _success, string _extendedInfo, MatchInfo _responseData)
        {
            Success = _success;
            ExtendedInfo = _extendedInfo;
            ResponseData = _responseData;
        }

    }
}