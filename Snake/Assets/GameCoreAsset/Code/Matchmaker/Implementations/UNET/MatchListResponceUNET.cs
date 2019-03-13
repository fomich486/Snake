using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.Match;

namespace MatchMaker
{
    public class MatchListResponceUNET : EventArgs, IMatchListResponce
    {
        public bool Success;
        public string ExtendedInfo;
        public List<MatchInfoSnapshot> ResponseData;

        public MatchListResponceUNET(bool _success, string _extendedInfo, List<MatchInfoSnapshot> _responseData)
        {
            Success = _success;
            ExtendedInfo = _extendedInfo;
            ResponseData = _responseData;
        }

        public IMatchInfo this[int i]
        {
            get
            {
                return new MatchInfoUNET(ResponseData[i]);
            }
        }

        public int Count
        {
            get
            {
                return ResponseData.Count;
            }
        }
    }
}