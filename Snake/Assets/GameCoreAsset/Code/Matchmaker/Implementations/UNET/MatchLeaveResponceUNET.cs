using System;

namespace MatchMaker
{
    public class MatchLeaveResponceUNET : EventArgs, IMatchLeaveResponce
    {
        public bool Success;
        public string ExtendedInfo;

        public MatchLeaveResponceUNET(bool _success, string _extendedInfo)
        {
            Success = _success;
            ExtendedInfo = _extendedInfo;
        }

    }
}