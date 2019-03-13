using DataSystem;
using System;

namespace MatchMaker
{
    public class LanListResponce : EventArgs, IMatchListResponce
    {
        private  LanMatchInfo matchInfo;
        public LanListResponce(string _fromAddress, string _data)
        {
            var _matchInfo = UnityEngine.JsonUtility.FromJson<LanMatchInfo>(_data);
            Console.Write(_data);
            UnityEngine.Debug.Log("Recieved => "+_data);
            matchInfo = new LanMatchInfo(_fromAddress, _matchInfo.AppId, _matchInfo.PlayersCount);
        }

        public IMatchInfo this[int i]
        {
            get
            {
                return matchInfo;
            }
        }

        public int Count { get {return matchInfo != null ? 1 : 0; } }
        
    }

    public class LanMatchInfo:IMatchInfo
    {
        public string FromAddress;
        public string AppId;
        public int PlayersCount;
        public LanMatchInfo(string _fromAddress, string _appId, int _playersCount)
        {
            FromAddress = _fromAddress;
            AppId = _appId;
            PlayersCount = _playersCount;
    }      
    } 
}