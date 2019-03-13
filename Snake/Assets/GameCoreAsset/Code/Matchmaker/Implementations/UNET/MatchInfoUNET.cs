using System.Collections.Generic;
using UnityEngine.Networking.Match;

namespace MatchMaker
{
    public class MatchInfoUNET:IMatchInfo
    {
        public MatchInfoSnapshot Info;

        public MatchInfoUNET(MatchInfoSnapshot _info)
        {
            Info = _info;
        }
    }
}